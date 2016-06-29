//
//  GameViewController.h
//  GLCamera01
//
//  Created by miyabi on 2015/12/30.
//  Copyright © 2015年 miyabi. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <GLKit/GLKit.h>
#import <AVFoundation/AVFoundation.h>

@interface GameViewController : GLKViewController <AVCaptureVideoDataOutputSampleBufferDelegate>
//@property (strong, nonatomic)UIViewController *vwcUnityView;
- (GLuint) getTexturePtr;
-(void)setGLCamera;
-(void) setBufferImage:(unsigned char *)data;
@end
