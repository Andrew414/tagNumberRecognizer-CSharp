import sys
import os

def filterImage(im):
	width, height = im.size
	pixels = im.load()
	for i in range(width):
		for j in range(height):
			curPixel = pixels[i,j]
			if max(curPixel) > min(curPixel) + threshold:
				print "Karaul!"
			if max(curPixel) < threshold:
				pixels[i,j] = (0,0,0)
			else:
				pixels[i, j] = (255, 255, 255)


import PIL

from PIL import Image


threshold = 100

def makeMatrixForSymbol(symbol):

	if not os.path.exists(symbol + ".bmp"):
		print "Find symbol " + symbol
		return
	im = Image.open(symbol + ".bmp")

	filterImage(im)

	width, height = im.size

	left = 0
	ready = False
	for i in range(width):
		if ready:
			break;
		for j in range(height):
			if im.getpixel((i,j)) == (0,0,0):
				ready = True
				left = i
				break

	right = width - 1
	ready = False
	for i in range(width - 1, 0, -1):
		if ready:
			break;
		for j in range(height):
			if im.getpixel((i,j)) == (0,0,0):
				ready = True
				right = i
				break

	up = height - 1
	ready = False
	for j in range(height - 1, 0, -1):
		if ready:
			break;
		for i in range(width):
			if im.getpixel((i,j)) == (0,0,0):
				ready = True
				up = j
				break

	down = height - 1
	ready = False
	for j in range(height):
		if ready:
			break;
		for i in range(width):
			if im.getpixel((i,j)) == (0,0,0):
				ready = True
				down = j
				break

	print left, right, up, down

	im = im.crop((left, down, right, up))

	im = im.resize((120, 200))

	if not os.path.exists(symbol):
		os.mkdir(symbol)

	matrixFile = open(os.path.join(symbol, symbol + ".txt"), "w")
	for i in range(120):
		for j in range(200):
			if im.getpixel((i,j)) == (0,0,0):
				matrixFile.write("1 ")
			else:
				matrixFile.write("0 ")
		matrixFile.write("\n")		

	im.save(os.path.join(symbol, symbol + "_resized+cropped.bmp"))

for i in range(10):
	makeMatrixForSymbol(str(i))

makeMatrixForSymbol('A')
makeMatrixForSymbol('B')
makeMatrixForSymbol('O')
makeMatrixForSymbol('T')
makeMatrixForSymbol('M')
makeMatrixForSymbol('E')
makeMatrixForSymbol('X')

