echo "Hello!"

echo "Installing the prerequisites"
sudo apt-get update
sudo apt-get install build-essential monodevelop libmono-wcf3.0-cil mono-gmcs libtiff5-dev libgeotiff-dev libgtk2.0-dev libgstreamer0.10-dev libavcodec-dev libswscale-dev libavformat-dev libopenexr-dev libjasper-dev libdc1394-22-dev libv4l-dev libqt4-opengl-dev libeigen2-dev libtbb-dev libtesseract-dev cmake-curses-gui git

echo "Fetching the Emcu CV"
git clone git://git.code.sf.net/p/emgucv/code ../emgucv

cd ../emgucv

echo "Initialize Emgu CV submodules, such as OpenCV"
git submodule update --init --recursive

echo "Make it, make it, oho-ho-ho! :)"
cmake -DBUILD_NEW_PYTHON_SUPPORT:BOOL=FALSE -DBUILD_TESTS:BOOL=FALSE -DBUILD_DOCS:BOOL=FALSE -DWITH_TBB:BOOL=TRUE -DWITH_CUDA:BOOL=FALSE -DWITH_OPENCL:BOOL=FALSE .

echo "Sit back and wait as this will build OpenCV, as well as cvextern.so, Emgu.Util.dll, Emgu.CV.dll, Emgu.CV.UI.dll and Emgu.CV.ML.dll"

make

cd bin

echo "Export path!"
export LD_LIBRARY_PATH=.:$LD_LIBRARY_PATH

echo "Test it!"
mono Example.PlanarSubdivision.monoexe

echo "If you see the planar subdivision window - everything is ok! If not - your computer will die in a minute! (:"
