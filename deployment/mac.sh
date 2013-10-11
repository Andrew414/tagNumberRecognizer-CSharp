echo "Hello!"

echo "You need to make sure that you have installed Mono, CMake and XCode!"

echo "Fetching the Emcu CV"
git clone git://git.code.sf.net/p/emgucv/code ../emgucv

cd ../emgucv

echo "Initialize Emgu CV submodules, such as OpenCV"
git submodule update --init --recursive

echo "Make it, make it, oho-ho-ho! :)"
cmake -DCMAKE_OSX_ARCHITECTURES=i386 -DBUILD_NEW_PYTHON_SUPPORT:BOOL=FALSE -DBUILD_PERF_TESTS=FALSE -DBUILD_TESTS:BOOL=FALSE -DBUILD_DOCS:BOOL=FALSE -DBUILD_JPEG=TRUE -DBUILD_PNG=TRUE -DBUILD_TIFF=TRUE .

echo "Sit back and wait as this will build OpenCV, as well as cvextern.so, Emgu.Util.dll, Emgu.CV.dll, Emgu.CV.UI.dll and Emgu.CV.ML.dll"

make

cd bin

echo "Test it!"
mono Example.PlanarSubdivision.monoexe

echo "If you see the planar subdivision window - everything is ok! If not - your computer will die in a minute! (:"
