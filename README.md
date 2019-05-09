# Sprite ToolBox
Sprite ToolBox is a tool for setting the origin, bounding box and animation speed of sprites. You can also setup anchors points that move from frame to frame. Sprite data is serialized to xml and saved to the same folder as the sprite (with the same filename but with the .xsprite extension). The data can be imported with the sprite into your game. A custom content pipeline is included for MonoGame.

![](https://github.com/MickGyver/SpriteToolBox/blob/master/images/screenshot.png)

## Features
* Set origin and bounding box for sprites graphically, even when the sprite is animated.
* Set speed of animation, specific frames can also have their own animation speed.
* Support for sprite strips (\_stripXX.png) and animated gifs.
* Drag and drop sprites to the sprite view.
* Sprite view can be zoomed and panned.
* Background color of sprite view can be changed.
* Set up anchors (points) on the sprite that move with the animation (also angle and length).

## Controls
* Cycle through sprite frames: MouseWheel.
* Zoom in and out: Control+MouseWheel.
* Pan around: Spacebar+LeftButton+Drag. To reset: Spacebar+RightButton
* Set origin: LeftButton at wanted position when origin is shown, or enter numeric values.
* Set bounding box: RightButton+Drag when bounding box is shown, or enter numeric values.
* Set anchor position for frame: LeftButton at wanted position when anchor is shown, or LeftButton+Drag to also set angle and length.
