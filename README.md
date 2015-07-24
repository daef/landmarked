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

rotate: move your mouse while holding the right mouse button
since the camera rotates around the point you've clicked
one can navigate through a mesh with right-clicks - try it out

zoom: mouse wheel

set # of landmarks: use the + and - buttons

set landmark: click somewhere on the mesh

remove landmark: click a landmark

remove all landmarks: click the [C] button

save landmarks in .pts format: click the [S] button

#### files

currently landmarked is able to read .stl and .pts files
simply drop them onto the canvas

#### known bugs

you might have to resize your browser window so the canvas get's the whole size

