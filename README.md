__landmarked__ is a minimal landmark editor

running in your browser (firefox tested/chrome should do too)

which replaces [landmark editor](http://www.idav.ucdavis.edu/research/projects/EvoMorph/) for me.

it's a small unity/webgl project containing:

* a .stl parser (reads binary and text .stl, does the unity-needed 65k splitting, merges vertices, normal calculator, uv calculator...)

* a sample camera navigation implementation

* a neat 3d cursor

### demo

try it out [here](http://mnml.is/landmarked) ( + [testdata](http://mnml.is/landmarked/testdata.zip) )

#### usage

__rotate__: move your mouse while holding the right mouse button.

__pan__: since the camera rotates around the point you've clicked you can navigate through the mesh with right-clicks - try it out - it's fun

__zoom__: mouse wheel

__set #__ of landmarks: use the + and - buttons

__add__ landmark: click on the mesh and it appears

__remove__ landmark: simply click the landmark you want to remove

__remove all__ landmarks: try the [C] button

__save__ landmarks in .pts format: finally, hit the [S] button

__load__ files: simply drop them on the canvas. landmarked understands .stl and .pts files

#### known bugs

you might have to resize your browser window so the canvas get's the whole size

