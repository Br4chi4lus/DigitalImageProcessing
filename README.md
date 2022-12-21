# DigitalImageProcessing
.Net WPF app that allows user various operations such as
- Edge detecion - Prewitt, Roberts Cross, Sobel, Scharr
- Blurring - low-pass filter, Gaussian blur (matrix size = 23, sigma = 5)
- Sharpening - high-pass filter
- Salt and pepper noise reduction - median filter
- Conversion to Grayscale - 24bit/8bit

App accepts 24-bit .bmp files only.
Processing can use mask of various sizes(currently sizes are hardcoded.
