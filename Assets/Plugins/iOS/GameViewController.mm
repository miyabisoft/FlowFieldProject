//
//  GameViewController.m
//  GLCamera01
//
//  Created by miyabi on 2015/12/30.
//  Copyright © 2015年 miyabi. All rights reserved.
//
#include <stdlib.h>
#include <stdint.h>
#import <opencv2/opencv.hpp>
#import <opencv2/highgui/ios.h>

#import "GameViewController.h"
#import <OpenGLES/ES2/glext.h>
#import <OpenGLES/ES2/gl.h>
#import <CoreVideo/CVOpenGLESTextureCache.h>
#import <CoreVideo/CoreVideo.h>
#import <CoreImage/CoreImage.h>
#import <CoreGraphics/CoreGraphics.h>

// Uniform index.
enum
{
    UNIFORM_Y,
    NUM_UNIFORMS
};
GLint uniforms[NUM_UNIFORMS];

@interface GameViewController () {
    GLKView *view;
    
    CGFloat _screenWidth;
    CGFloat _screenHeight;
    
    EAGLContext *_context;
    CVOpenGLESTextureRef _lumaTexture;
    // GLuint 型で生成されるテクスチャオブジェクト
    CVOpenGLESTextureCacheRef _videoTextureCache;
    void*	cvTextureCacheTexture;
    
    CVImageBufferRef pixelBuffer;
    NSString *_sessionPreset;
    AVCaptureSession *_session;
    bool flashon;
    int width;
    int height;
    unsigned char *bb;
    bool isImageSet;
    UIImage *mImage;
}

void CheckGLESError(const char* file, int line);
#if ENABLE_UNITY_GLES_DEBUG
#define GLESAssert()	do { CheckGLESError (__FILE__, __LINE__); } while(0)
#define GLES_CHK(expr)	do { {expr;} GLESAssert(); } while(0)
#else
#define GLESAssert()	do { } while(0)
#define GLES_CHK(expr)	do { expr; } while(0)
#endif

@end

@implementation GameViewController
NSTimer *stroboTimer;

-(void)startCamera
{
    [self setGLCamera];
}

-(BOOL)shouldAutorotate{
    return(NO);
}

-(void)setGLCamera
{
    _videoTextureCache = 0;
    cvTextureCacheTexture = 0;
    isImageSet = false;
    
    _context = [EAGLContext currentContext];
    
    if (!_context) {
        NSLog(@"Failed to create ES context");
    }
    
    view = (GLKView *)self.view;
    view.context = _context;
    self.preferredFramesPerSecond = 60;
    
    _screenWidth = [UIScreen mainScreen].bounds.size.width;
    _screenHeight = [UIScreen mainScreen].bounds.size.height;
    
    view.contentScaleFactor = [UIScreen mainScreen].scale;
    
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
    {
        // meshFactor controls the ending ripple mesh size.
        // For example mesh width = screenWidth / meshFactor.
        // It's chosen based on both screen resolution and device size.
        // Choosing bigger preset for bigger screen.
        _sessionPreset = AVCaptureSessionPreset1280x720;
    }
    else
    {
        _sessionPreset = AVCaptureSessionPreset640x480;
    }
    
    //glUniform1i(uniforms[UNIFORM_Y], 0);

   [self setupAVCapture];
}

// タスク起動
- (void)viewDidLoad
{
    [super viewDidLoad];
    [self setGLCamera];
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    
    [self tearDownAVCapture];
    
    if ([EAGLContext currentContext] == _context) {
        [EAGLContext setCurrentContext:nil];
    }
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Release any cached data, images, etc. that aren't in use.
}

- (void)cleanUpTextures
{
    if (_lumaTexture)
    {
        CFRelease(_lumaTexture);
        _lumaTexture = NULL;
    }
    // Periodic texture cache flush every frame
    CVOpenGLESTextureCacheFlush(_videoTextureCache, 0);
}

// GLES mode
/* 以下がビデオフレームデータを得るためのデリゲートメソッドです。最初にサンプルバッファからピクセルバッファ（CVPixelBufferRef）を得ます。その後、CVOpenGLESTextureCacheCreateTextureFromImage()ルーチンでOpenGLテクスチャ（CVOpenGLESTextureRef）を作り、それを使いglBindTexture()を実行してから、glTexParameteri()でテクスチャの表示用パラメータを設定します。*/
- (void)captureOutput:(AVCaptureOutput *)captureOutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer
       fromConnection:(AVCaptureConnection *)connection
{
    CVReturn err;

    if (isImageSet == true)
        return;
    
    pixelBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);

    size_t width = CVPixelBufferGetWidth(pixelBuffer);
    size_t height = CVPixelBufferGetHeight(pixelBuffer);
    if (!_videoTextureCache)
    {
        NSLog(@"No video texture cache");
        return;
    }
    [self cleanUpTextures];
    
    mImage = [self imageFromSampleBufferRef:sampleBuffer];
    
    // CVOpenGLESTextureCacheCreateTextureFromImage will create GLES texture
    // optimally from CVImageBufferRef.
    
    // Y-plane
    // CVPixelBufferRefからテクスチャを作成（Core Video）
    // UV-plane
    // CVPixelBufferRefからテクスチャを作成（Core Video）GL_RED_EXT GL_TEXTURE_2D (GL_RGBA / GL_BGRA)
/*
    glActiveTexture(GL_TEXTURE0);
    assert(glGetError() == GL_NO_ERROR);
    err = CVOpenGLESTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                       _videoTextureCache,
                                                       pixelBuffer,
                                                       NULL,
                                                       GL_TEXTURE_2D,
                                                       GL_RGBA,
                                                       width,
                                                       height,
                                                       GL_BGRA,
                                                       GL_UNSIGNED_BYTE,
                                                       0,
                                                       &_lumaTexture);
    if (err)
    {
        NSLog(@"Error at CVOpenGLESTextureCacheCreateTextureFromImage %d", err);
    }
    
    //NSLog(@"glBindTextureID %d", CVOpenGLESTextureGetName(_lumaTexture));
    
    // OpenGLテクスチャの使用宣言とテクスチャ用パラメータの設定
    glBindTexture(CVOpenGLESTextureGetTarget(_lumaTexture), CVOpenGLESTextureGetName(_lumaTexture));
    assert(glGetError() == GL_NO_ERROR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glBindTexture(CVOpenGLESTextureGetTarget(_lumaTexture), 0);
    assert(glGetError() == GL_NO_ERROR);
 */
    isImageSet = true;
}

// UIImage -> cv::Mat変換
-(UIImage *) UIImageToMat:(UIImage *)data
{
    CGColorSpaceRef colorSpace = CGImageGetColorSpace(data.CGImage);
    CGFloat cols = data.size.width;
    CGFloat rows = data.size.height;
    
    cv::Mat mat(rows, cols, CV_8UC4);
    CGContextRef contextRef = CGBitmapContextCreate(mat.data,
            cols,rows,8,mat.step[0],colorSpace,
            kCGImageAlphaNoneSkipLast |kCGBitmapByteOrderDefault);
    CGContextDrawImage(contextRef, CGRectMake(0, 0, cols, rows), data.CGImage);
    CGContextRelease(contextRef);
    // cv::May -> UIImage変換
    UIImage *resultImage = MatToUIImage(mat);
    return resultImage;
}

-(void) setBufferImage:(unsigned char *)data
{
    if(isImageSet == true) {
        // UIImage -> cv::Mat変換
        CGColorSpaceRef colorSpace = CGImageGetColorSpace(mImage.CGImage);
        CGFloat cols = mImage.size.width;
        CGFloat rows = mImage.size.height;
        //NSLog(@"cols = %f", cols);
        //NSLog(@"rows = %f", rows);
        
        cv::Mat mat(rows, cols, CV_8UC4);
        CGContextRef contextRef = CGBitmapContextCreate(mat.data,
                     cols,rows,8,mat.step[0],colorSpace,
                     kCGImageAlphaNoneSkipLast |kCGBitmapByteOrderDefault);
        CGContextDrawImage(contextRef, CGRectMake(0, 0, cols, rows), mImage.CGImage);
        CGContextRelease(contextRef);
        
        // cv::May -> UIImage変換
        //UIImage *resultImage = MatToUIImage(mat);
        //UIImageWriteToSavedPhotosAlbum(resultImage, nil, nil, nil);
        
        // Unity2D テクスチャーに画像を流し込む
        memcpy(data, mat.data, mat.total()*mat.elemSize());
        
        mat.release();
        isImageSet = false;
    }
}

// CMSampleBufferRefをUIImageへ
- (UIImage *)imageFromSampleBufferRef:(CMSampleBufferRef)sampleBuffer
{
    // イメージバッファの取得
    CVImageBufferRef    buffer;
    buffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    
    // イメージバッファのロック
    CVPixelBufferLockBaseAddress(buffer, 0);
    // イメージバッファ情報の取得
    uint8_t*    base;
    size_t      width, height, bytesPerRow;
    base = (uint8_t*)CVPixelBufferGetBaseAddress(buffer);
    width = CVPixelBufferGetWidth(buffer);
    height = CVPixelBufferGetHeight(buffer);
    bytesPerRow = CVPixelBufferGetBytesPerRow(buffer);
    
    // ビットマップコンテキストの作成
    CGColorSpaceRef colorSpace;
    CGContextRef    cgContext;
    colorSpace = CGColorSpaceCreateDeviceRGB();
    cgContext = CGBitmapContextCreate(
                                      base, width, height, 8, bytesPerRow, colorSpace,
                                      kCGBitmapByteOrder32Little | kCGImageAlphaPremultipliedFirst);
    CGColorSpaceRelease(colorSpace);
    
    // 画像の作成
    CGImageRef  cgImage;
    UIImage*    image;
    cgImage = CGBitmapContextCreateImage(cgContext);
    image = [UIImage imageWithCGImage:cgImage scale:1.0f
                          orientation:UIImageOrientationUp];
    CGImageRelease(cgImage);
    CGContextRelease(cgContext);
    
    // イメージバッファのアンロック
    CVPixelBufferUnlockBaseAddress(buffer, 0);
    return image;
}

-(GLuint) getTexturePtr
{
    int textid = CVOpenGLESTextureGetName(_lumaTexture);
    return CVOpenGLESTextureGetName(_lumaTexture);
}

-(void)didRotateFromInterfaceOrientation:(UIInterfaceOrientation)FromInterfaceOrientation {
    if(FromInterfaceOrientation == UIInterfaceOrientationPortrait){
        // 横向き
    } else {
        // 縦向き
    }
}
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return YES;
}

- (void)setupAVCapture
{
    NSDictionary    *rgbOutputSettings;
    AVCaptureConnection *videoConnection = NULL;
    
    //-- Create CVOpenGLESTextureCacheRef for optimal CVImageBufferRef to GLES texture conversion.
#if COREVIDEO_USE_EAGLCONTEXT_CLASS_IN_API
    CVReturn err = CVOpenGLESTextureCacheCreate(kCFAllocatorDefault, NULL, _context, NULL, &_videoTextureCache);
#else
    CVReturn err = CVOpenGLESTextureCacheCreate(kCFAllocatorDefault, NULL, (__bridge void *)_context, NULL, &_videoTextureCache);
#endif
    if (err)
    {
        NSLog(@"Error at CVOpenGLESTextureCacheCreate %d", err);
        return;
    }
    
    //-- Creata a video device and input from that Device.  Add the input to the capture session.
    AVCaptureDevice * videoDevice = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if(videoDevice == nil)
        assert(0);
    
    //-- Add the device to the session.
    NSError *error;
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:videoDevice error:&error];
    if(error)
        assert(0);
    
    //-- Create the output for the capture session.
    AVCaptureVideoDataOutput * dataOutput = [[AVCaptureVideoDataOutput alloc] init];
    [dataOutput setAlwaysDiscardsLateVideoFrames:YES]; // Probably want to set this to NO when recording
    
    //　出力映像フォーマット（32BGRA）kCVPixelFormatType_32BGRA
    //     kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange
    //     kCVPixelFormatType_420YpCbCr8BiPlanarFullRange
    
    rgbOutputSettings=[NSDictionary
                       dictionaryWithObject:[NSNumber numberWithInt:kCVPixelFormatType_32BGRA]
                       forKey:(id)kCVPixelBufferPixelFormatTypeKey];
    //　ビデオフォーマット設定
    [dataOutput setVideoSettings:rgbOutputSettings];
    //　逐次ビデオフレームを破棄
    [dataOutput setAlwaysDiscardsLateVideoFrames:YES];
    
    //-- Set to YUV420.
    //  [dataOutput setVideoSettings:[NSDictionary dictionaryWithObject:[NSNumber numberWithInt:kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange]
    //                                                        forKey:(id)kCVPixelBufferPixelFormatTypeKey]]; // Necessary for manual preview
    
    // Set dispatch to be on the main thread so OpenGL can do things with the data
    [dataOutput setSampleBufferDelegate:self queue:dispatch_get_main_queue()];
    
    //-- Setup Capture Session.
    _session = [[AVCaptureSession alloc] init];
    [_session beginConfiguration];
    
    //-- Set preset session size.
    [_session setSessionPreset:_sessionPreset];
    [_session setSessionPreset:AVCaptureSessionPresetMedium];
    [_session setSessionPreset:AVCaptureSessionPresetPhoto];
    [_session addInput:input];
    [_session addOutput:dataOutput];
    
    for(AVCaptureConnection *connection in dataOutput.connections)
    {
        if(connection.supportsVideoOrientation)
        {
            //connection.videoOrientation = videoOrientationFromDeviceOrientation([UIDevice currentDevice].orientation);
            connection.videoOrientation = AVCaptureVideoOrientationPortraitUpsideDown;
        }
    }
    
    [_session commitConfiguration];
    
    // トーチOFF
   [self torchOffFunction];
    
    flashon = false;
   [self setStroboTimer];

    [_session startRunning];
    
}

int cunt;
-(void)setStroboTimer
{
    if (!stroboTimer) {
        cunt = 0;
        stroboTimer = [NSTimer scheduledTimerWithTimeInterval:0.01 target:self selector:@selector(timerStrobo:) userInfo:nil repeats:YES];
        [stroboTimer fire];
    }
}

-(void)timerStrobo:(NSTimer *)timer
{
    if (cunt > 1000) {
        flashon = true;
        [self StoroboSwitch];
        [timer invalidate];
        stroboTimer = nil;
    }else{
        [self StoroboSwitch];
        cunt++;
    }
}

// ストロボ
-(void)StoroboSwitch
{
    AVCaptureDevice *flashLight = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if([flashLight isTorchAvailable] && [flashLight isTorchModeSupported:AVCaptureTorchModeOff])
    {
        BOOL success = [flashLight lockForConfiguration:nil];
        if(success)
        {
            if (flashon == false) {
                [flashLight setTorchMode:AVCaptureTorchModeOn];
                flashon = true;
            }else if (flashon == true) {
                [flashLight setTorchMode:AVCaptureTorchModeOff];
                flashon = false;
            }
            [flashLight unlockForConfiguration];
        }
    }
}

// トーチON
- (void)torchOnFunction
{
    AVCaptureDevice *flashLight = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if([flashLight isTorchAvailable] && [flashLight isTorchModeSupported:AVCaptureTorchModeOn])
    {
        BOOL success = [flashLight lockForConfiguration:nil];
        if(success)
        {
            [flashLight setTorchMode:AVCaptureTorchModeOn];
            [flashLight unlockForConfiguration];
        }
    }
}

// トーチOFF
- (void)torchOffFunction
{
    AVCaptureDevice *flashLight = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if([flashLight isTorchAvailable] && [flashLight isTorchModeSupported:AVCaptureTorchModeOff])
    {
        BOOL success = [flashLight lockForConfiguration:nil];
        if(success)
        {
            NSLog(@"torchOffFunction");
            [flashLight setTorchMode:AVCaptureTorchModeOff];
            [flashLight unlockForConfiguration];
        }
    }
}

static AVCaptureVideoOrientation videoOrientationFromDeviceOrientation(UIDeviceOrientation deviceOrientation)
{
    AVCaptureVideoOrientation orientation;
    switch (deviceOrientation) {
        case UIDeviceOrientationUnknown:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationPortrait:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationPortraitUpsideDown:
            orientation = AVCaptureVideoOrientationPortraitUpsideDown;
            break;
        case UIDeviceOrientationLandscapeLeft:
            orientation = AVCaptureVideoOrientationLandscapeRight;
            break;
        case UIDeviceOrientationLandscapeRight:
            orientation = AVCaptureVideoOrientationLandscapeLeft;
            break;
        case UIDeviceOrientationFaceUp:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
        case UIDeviceOrientationFaceDown:
            orientation = AVCaptureVideoOrientationPortrait;
            break;
    }
    return orientation;
}

- (void)tearDownAVCapture
{
    [self cleanUpTextures];
    
    CFRelease(_videoTextureCache);
}



@end
