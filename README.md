# MAX7456 Font Editor
A simple, easy to use font editor for the MAX7456.

With this, you can easily edit the font file directly, export the characters or import individual or a set of characters. This allows you to easily edit and clean up fonts in an application such as photoshop.

## File Formats
The editor allows easy opening and saving of the ATMega/Arduino "MAX7456 font uploader" style programs which are readily available. The header files can be easily loaded and saved without requiring any conversion utility, or the files from the Maxim editor (.mcm) can be loaded and saved. This allows you to also easily convert between the two formats.

## The Project
This is a program I created over a year ago and have finally gotten around to sharing. I spent a few hours writing this and have not modified it since. It's performance is not stellar but it is very usable. Basically, it was quick to write and is functional. When multiple fonts are tiled in the editor area, performance can slow down quite a bit.

## Workflow 
The preferred workflow is to open the font file, save an image out which you want to edit, or create a new image in photoshop. Any non-black/white pixel is considered transparent background when importing.

If you are editing multiple character symbols (such as arrows or battery bars) you can easily tile the images on screen by multiple selecting them from the fonts pane (ctrl+click or shift+click). You can also import tiled images from a single image file.
