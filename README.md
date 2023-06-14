# Voxel Path Tracer
A CPU-only path tracer for rendering voxel grids.

![Preview](doc/preview.png)

* [Features](#features)
* [Future ideas](#future-ideas)
* [Examples](#examples)
    - [BasicExample](#basicexample)
* [Dev log](#dev-log)
    - [1. Diffuse color](#1-diffuse-color)
    - [2. Emissive surfaces](#2-emissive-surfaces)
    - [3. Hard shadows](#3-hard-shadows)
    - [4. Reflections](#4-reflections)
    - [5. Soft shadows](#5-soft-shadows)

## Features
* Unidirectional path tracing
* Based on fast voxel traversal algorithm (DDA)
* Perspective camera
* Multiple sampling
* Supports color, emission and reflectivity
* Sun with direction and color
* Simple API
* No thrid party dependencies

## Future ideas
* Transparency support

## Examples
#### BasicExample
Rnders an image with some predefined settings.

####MengerSpongeExample
Renders a menger sponge (like in the picture above). The program is executed using command line and has 
the following options:
```
 -i, --iterations    (Default: 3) Number of iterations in range [1, 6]
 -s, --samples       (Default: 300) Number of render samples
 -r, --resolution    (Default: 500) Resolution of rendered image
 -p, --path          (Default: image.png) Path where rendered image will be saved
 -l, --light         (Default: false) Add a light in the middle of the sponge
 -c, --color         (Default: Cyan) White, Gray, Black, Cyan, Yellow, Red, Green, Blue
 -d, --denoiser      (Default: false) Use median filter to reduce noise
 --help              Display the help screen.
 --version           Display version information.
```

## Dev log
#### 1. Diffuse color
<img src="log/3.png" width="200" height="200" alt="Diffuse">

#### 2. Emissive surfaces
<img src="log/6.png" width="200" height="200" alt="Emissive1">
<img src="log/5.png" width="200" height="200" alt="Emissive2">

#### 3. Hard shadows
<img src="log/8.png" width="200" height="200" alt="Hard shadows">

#### 4. Reflections
<img src="log/9.png" width="200" height="200" alt="Mirror">
<img src="log/10.png" width="200" height="200" alt="Fuzzy">

#### 5. Soft shadows
<img src="log/11.png" width="200" height="200" alt="Soft shadows">
