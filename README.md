# DigitalImageProcessing
.Net WPF app that allows user various operations such as
- Edge detecion - Prewitt, Roberts Cross, Sobel, Scharr
- Blurring - low-pass filter(5 different masks), Gaussian blur(it is possible to pick sigma and matrix size)
- Sharpening - high-pass filter(4 different masks)
- Salt and pepper noise reduction - median filter
- Conversion to Grayscale - 24bit/8bit

App accepts 24-bit .bmp files only.
Processing can use mask of various sizes(currently sizes are hardcoded except gaussian blur).
