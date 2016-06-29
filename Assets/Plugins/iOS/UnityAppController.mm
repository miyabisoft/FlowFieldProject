//
//  UnityAppController.m
//  GLCamera01Lib
//
//  Created by miyabi on 2016/04/14.
//  Copyright © 2016年 miyabi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GameViewController.h"

GameViewController *gameViewController;
extern UIViewController* UnityGetGLViewController();

extern "C" {
    void startGLCamera();
    GLuint getTexturePtr();
    void setImageBuffer(unsigned char* data);
}

void startGLCamera()
{
    //UIViewController* parent = UnityGetGLViewController();
    gameViewController = [[GameViewController alloc] init];
    [gameViewController  setGLCamera ];
    //[parent.view addSubview:gameViewController.view];
    //gameViewController.view.hidden = NO;
    
    // 重なり順を最前面に
    //[parent.view bringSubviewToFront:gameViewController.view];
    // 重なり順を最背面に
   // [parent.view sendSubviewToBack:gameViewController.view];
}

GLuint getTexturePtr()
{
    return  [gameViewController  getTexturePtr ];
}

void setImageBuffer(unsigned char* data)
{    
    [gameViewController setBufferImage:data];
}
