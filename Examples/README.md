Example Applications
====================

How-to Examples
---------------
Three of these example applications demonstrate how to use the QuickFIX/n library to
build your own FIX applications.  They are:

* **SimpleAcceptor:** demonstrates a barebones acceptor application.
* **Executor:** takes orders over a FIX session and "executes" them.
* **TradeClient:** a command-line client that sends orders.  _(NOT FOR USE WITH COMMERCIAL FIX INTERFACES!  It won't work!)_

TradeClient and Executor can be configured to send and execute
orders with each other.

TradeClient can also be configured to connect to SimpleAcceptor,
though TradeClient will not hear any application-level responses from
SimpleAcceptor.

To run each, go into its directory and use `dotnet run <config>`, e.g.
* `Examples/SimpleAcceptor> dotnet run simpleacc.cfg`
* `Examples/Executor> dotnet run executor.cfg`
* `Examples/TradeClient> dotnet run tradeclient.cfg`

### A note about SSL variant configurations

Many of the following applications have alternate `*_ssl.cfg`
files that you can use to demonstrate secure communication over SSL.

**However**, these configs point to certificats that are only valid for a few days.
You will need to generate new certificates.
You can do this by using the included `GenerateKeys` app.

```
~/quickfixn/GenerateKeys$ dotnet run ../Examples/
Writing CACertificate: ../Examples/QuickFixn-TestCA.cer
Writing ServerCertificate: ../Examples/QuickFixn-TestServer.pfx
Writing ClientCertificate: ../Examples/QuickFixn-TestClient.pfx
```

That's it!  Now you can run examples using `*_ssl.cfg` configs.


### SimpleAcceptor

The SimpleAcceptor example shows you how to create a simple acceptor server.
It will let initiators connect to it, and logs all admin and application
level messages to the screen. It does not process these messages.

Program.cs demonstrates how to setup and start a new acceptor object
from the Session settings file. SimpleAcceptorApp.cs implements
the Application interface and is where you would handle all your application
level logic.

Configure the SimpleAcceptor by modifying SimpleAcceptor/simpleacc.cfg
This configuration file defines the FIX Sessions the acceptor will
handle. For more information:  
http://quickfixn.org/tutorial/configuration


### Executor

The Executor example takes incoming orders and executes them. Executor demonstrates
how to create an acceptor to crack messages and execute orders.
The Executor class inherits MessageCracker and implements Application.

For more information on how message cracking works:  
http://quickfixn.org/tutorial/receiving-messages

The OnMessage callbacks show you how to get field values from the NewOrderSingle 
FIX message and create an ExecutionReport response back to the initiator. Every
FIX version has its own namespace and Messages. When responding to a NewOrderSingle,
you must create a Message based on the initiator's FIX version. If you do not,
the counterparty will reject your message.

The Executor is configured with the executor.cfg file.


### TradeClient

The TradeClient is a command line example that shows how to create different FIX 
message types and versions. You can create new order singles, cancel order requests, 
cancel replace requests, and market data requests. 

DO NOT ATTEMPT TO CONNECT TRADECLIENT WITH A COMMERCIAL FIX INTERFACE!  It's a
terrible idea and it will not work.  TradeClient is just an example, to give you a
demonstration of how to complete certain tasks in your own application.

TradeClient is configured with the tradeclient.cfg file.


FIX/Json Examples
-----------------

These demonstrate the FIX-to-Json and Json-to-FIX conversions.  They are pretty simple.


Standalone/SerilogLog
---------------------

See the README in that directory.
