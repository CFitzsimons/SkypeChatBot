SkypeChatBot
============
A simple chat bot for Skype.  

Useage
======

Command.xml is the file that contains the commands you wish to enable on the bot.  Two commands are provided by default, outside the XML, they are documented below.  In order to add a new command a new entry into the XML should be added.  The format of the file is as follows:
```
<CommandList>
  <command>
    <trigger>Hello</trigger>
    <message>Hello yourself!</message>
    <timer>3</timer>
    <file>C:/text.txt</file>
  </command>
</CommandList>
```

```
  <CommandList>
  This is the root element, containing a list of commands.
```
```
  <command>
  This is the root element for an individule command, there can be many <command> elements.
```
```
  <trigger>
  This is the word that triggers the parameters you have set.  In the above exmaple 
  the trigger would be !Hello being input into a Skype chat.
```
```
  <message>
  This is the message that will be displayed when an action has been triggered.
```
```
  <timer>
  More relevent for the file field, this will set a delay in second before the 
  command is dumped.  In terms of a file, it will be the time inbetween each line.  
  This field can be left out and a value of 0 is assumed.  
```
```
  <file>
  Path to a file that can be read in a used to respond, line by line, to a trigger.  
  This field can be left out.
```

Additional Commands
===================

!Timer X Message
--------

This will set a timer for X minutes, upon reaching X minutes past - a message will be displayed corresponding to the message set.  For example:
    !Timer 3 Hello World
would display the message "Hello World" three minutes after the timer was set.  

!Random low high
----------------

This will generate a random number between the base value (low) and the max value (high).  For example:
    !Random 0 3
Could generate 0, 1 or 2

Download
========
You can grab the executable from the .io website.
