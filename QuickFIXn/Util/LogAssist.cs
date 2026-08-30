using System.Collections.Generic;
using System.Text;

namespace QuickFix.Util;

internal class LogAssist
{
    internal static string PrepareFixMessageForLog(string msg, Session sesh)
    {
        if (string.IsNullOrEmpty(msg) || string.IsNullOrEmpty(msg))
            return msg;
        if (sesh.RedactFieldsInLogs.Length > 0)
            msg = RedactSensitiveFields(msg, sesh.RedactFieldsInLogs, sesh.RedactionLogText);
        msg = SwapFieldSeparator(msg, sesh.FieldSeparatorInMessageLogs);

        return msg;
    }

    internal static string SwapFieldSeparator(string msg, char newSeparator)
    {
        if (newSeparator == Message.SOH)
            return msg;
        return msg.Replace(Message.SOH, newSeparator);
    }

    /// <summary>
    /// Don't call this after SwapFieldSeparator!
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="tagsToRedact"></param>
    /// <param name="redactionText"></param>
    /// <returns></returns>
    internal static string RedactSensitiveFields(string msg, int[] tagsToRedact, string redactionText = "<redacted>")
    {
        // This long text processor is much faster than Regex.
        // Since this is used on every message log, it needs to be fast.

        if (tagsToRedact.Length == 0 || string.IsNullOrEmpty(msg))
            return msg;

        var sensitiveTags = new HashSet<int>(tagsToRedact);
        StringBuilder? sb = null;

        int len = msg.Length;
        int lastCopiedPos = 0;
        int currentPos = 0;

        while (currentPos < len)
        {
            // 1. Identify the tag start (skip leading SOH if present)
            int tagStart = currentPos;
            if (msg[currentPos] == Message.SOH)
                tagStart++;

            // 2. Find the '=' delimiter
            int eqPos = -1;
            for (int i = tagStart; i < len; i++)
            {
                if (msg[i] == '=')
                {
                    eqPos = i;
                    break;
                }
                if (msg[i] == Message.SOH)
                    break; // Malformed field
            }

            if (eqPos == -1)
                break;

            // 3. Parse tag as integer (no substring allocation)
            int tag = 0;
            bool validTag = tagStart != eqPos;
            for (int i = tagStart; i < eqPos; i++)
            {
                char c = msg[i];
                if (c is >= '0' and <= '9')
                    tag = tag * 10 + (c - '0');
                else
                {
                    validTag = false;
                    break;
                }
            }

            // 4. Find field end (next SOH or end of string)
            int nextSoh = -1;
            for (int i = eqPos + 1; i < len; i++)
            {
                if (msg[i] == '\x01')
                {
                    nextSoh = i;
                    break;
                }
            }
            int fieldEnd = nextSoh == -1 ? len : nextSoh;

            // 5. Redact if sensitive
            if (validTag && sensitiveTags.Contains(tag))
            {
                sb ??= new StringBuilder(len);

                // Append everything up to '='
                sb.Append(msg, lastCopiedPos, eqPos + 1 - lastCopiedPos);
                sb.Append(redactionText);

                lastCopiedPos = fieldEnd;
            }

            currentPos = fieldEnd;
        }

        if (sb == null)
            return msg;

        // Append remaining part of the message
        if (lastCopiedPos < len)
            sb.Append(msg, lastCopiedPos, len - lastCopiedPos);

        return sb.ToString();
    }
}
