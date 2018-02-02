# StringToXML
A module to save string data to a file and read it back.

This is a Crestron module designed to write data to an XML file. it can be any sorty of string data like phone numbers, TV channels, or configuration data.

The file name parameter governs where the file is located and it's name.
The RecordLabel parameter changes the internal <element></element> tags in hte XML file. It's fine to leave it as the default "Record"

on boot, the file is read and the outputs are populated from the file. If no fileexists yet, all outputs are null.
When any of the inputs change, the file is re-written, read back and the outputs populated.

Included in the example files is a module that accepts strings and pulses dialer digitals. Can be used to dial phone conference presets or TV tuner channels.
