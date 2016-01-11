# Crython
Crython is a subset of Python, in particular, it is the intersection of features of Python and [Crayon](https://github.com/blakeohare/crayon). Crython is translated into both Python and Crayon. This is useful if you know Python but would like to leverage the platform support of Crayon (JavaScript, C#, Java). 

Crython is intended for games. Python output uses the PyGame library. Output is valid in both Python 2.x and 3.x.

Features:
- basic OO
- control flow:
-- for loops
-- while loops
-- if/elif/else statements
-- continue/break
- dictionaries and lists, (no tuples or sets)
- basic numbers (no complex)
- strings (single, double, and triple quoted strings)

No Python built in functions or methods are allowed. You must invoke one of the built-in Crython methods which are denoted with a dollar sign ($) prefix.

## Dictionaries
- $dictionary_get_with_default(dictionary, key, fallback_value) - note that directly fetching a value with square brackets is still allowed.
- $dictionary_keys(dictionary) - return a list of keys
- $dictionary_size(dictionary) - returns the size of the dictionary

## Lists
- $list_add(list, value) - appends a value to the end of a list
- $list_join(list, sep) - joins a list into a string using the given separator
- $list_length(list) - returns the length of a list
- $list_remove(list, inddex) - removes an item from a list
- $list_shuffle(list) - shuffles a list in place (no return value)

## Math
- $math_abs(num) - returns the absolute value of a number
- $math_sin(theta) - returns the sine of a value

## Numbers
- $int(num) - floors an integer
- $parse_int(str) - converts a string into an integer

## Print
- $print(value) - prints a value to STDOUT

## Resources
- $read_text_resource(path) - reads an embedded text resource as a string

## Strings
- $str(value) - converts a value to a string
- $string_length(str) - returns the length of a string
- $string_lower(str) - returns a lowercase version of a string
- $string_split(str, sep) - splits a string along the given delimiter
- $string_trim(str) - trims whitespace from both sides of a string
- $string_upper(str) - returns an uppercase version of a string

Additionally, the following graphical/game-related functions are provided

## Geometric Primitives
- $draw_ellipse(screen, left, top, width, height, red, green, blue) - draws a ellipse on the screen
- $draw_rectangle(screen, left, top, width, height, red, green, blue) - draws a rectangle on the screen
- $draw_triangle(screen, x1, y1, x2, y2, x3, y3, red, green, blue) - draws a triangle on the screen

## Images
- $image_blit(screen, image, x, y) - draws an image on the screen where the top left corner is position at (x,y)
- $image_blit_partial(screen, image, x, y, sourceX, sourceY, width, height) - draws a part of an image with the top left corner positioned at (x,y). The segment of the image is defined as the rectangle with the top left corner on the source image at (sourceX, sourceY) and has a size of width x height.
- $image_height(image) - returns the height of an image.
- $image_scale(image, new_width, new_height) - scales an image to the new dimensions.
- $image_width(image) - returns the width of an image.

## Sound
- $music_play(filename) - plays a song

## Screen
- $screen_fill(screen, red, green, blue) - fills the screen with the given color
