#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

typedef void (*INT_CALLBACK)(int);

@interface BackendWebView: NSObject <UIAlertViewDelegate, WKNavigationDelegate, WKScriptMessageHandler>
{
    INT_CALLBACK alertCallBack;
    NSDate *creationDate;
    INT_CALLBACK shareCallBack;
    WKWebView *webView;
    
    NSString* clientAppId;
    NSString* gamerIndate;
    
    NSString* htmlPath;
    NSString* indexHtml;
    NSString* listHtml;
}
@end

@implementation BackendWebView

static BackendWebView *instance;

+(BackendWebView*) instance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[BackendWebView alloc] init];
    });
    return instance;
}

-(id)init
{
    self = [super init];
    if (self)
        NSLog(@"TheBackendQuestionView was created");
    return self;
}

-(void)openWebView:(NSString*)_path gameId:(NSString*)_gameId indate:(NSString*)_gamerIndate left:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
    if (webView!=nil)
    {
        NSLog(@"WebView is already created");
        return;
    }
    UIView *mainView = UnityGetGLView();
    
    
    clientAppId = _gameId;
    gamerIndate = _gamerIndate;
    htmlPath = _path;

    WKUserContentController *wkusercontentcontroller = [[WKUserContentController alloc] init];
    [wkusercontentcontroller addScriptMessageHandler:self name:@"iosMessage"];
    [wkusercontentcontroller addScriptMessageHandler:self name:@"consoleMessage"];
    
    WKWebViewConfiguration *configuration = [[WKWebViewConfiguration alloc] init];
    configuration.userContentController = wkusercontentcontroller;
    [configuration setValue:@TRUE forKey:@"allowUniversalAccessFromFileURLs"];

    
    CGRect frame = mainView.frame;
    frame.origin.y += top;
    frame.size.height -= top;
    frame.size.height -= bottom;
    
    frame.origin.x += left;
    frame.size.width -= left;
    frame.size.width -= right;

    webView = [[WKWebView alloc] initWithFrame:frame configuration:configuration];
    webView.navigationDelegate = self;
    
    NSString *indexFileName = @"BackendQuestionMain.html";
    indexHtml = [NSString stringWithFormat:@"%@%@",htmlPath,indexFileName];
    
    NSString *listFileName = @"BackendQuestionList.html";
    listHtml = [NSString stringWithFormat:@"%@%@",htmlPath,listFileName];
    
    NSURLRequest *req = [NSURLRequest requestWithURL:[NSURL fileURLWithPath:indexHtml]];
    
    
    [webView loadRequest:req];
    [mainView addSubview:webView];
    
}
- (void)userContentController:(WKUserContentController*)userContentController didReceiveScriptMessage:(WKScriptMessage*)message
{
    if([message.name isEqualToString:@"consoleMessage"])
    {
        NSLog(@"console.log: %@",message.body);
        return;
    }
    if(![message.name isEqualToString:@"iosMessage"])
    {
        return;
    }
    NSString * order = message.body;
    
    if([order isEqualToString: @"ChangeIndexScene"])
    {
        NSURLRequest *req = [NSURLRequest requestWithURL:[NSURL fileURLWithPath:indexHtml]];
        [webView loadRequest:req];

        
    }
    else if([order isEqualToString: @"ChangeListScene"])
    {
        NSURLRequest *req = [NSURLRequest requestWithURL:[NSURL fileURLWithPath:listHtml]];
        [webView loadRequest:req];

    }
    else if([order isEqualToString:@"SettingIosInfo"])
    {
        NSString *filename = [NSString stringWithFormat:@"iOSSetGameIdAndGamerIndate(\"%@\",\"%@\")",clientAppId,gamerIndate];
        [webView evaluateJavaScript:filename completionHandler:nil];
    }
    else if([order isEqualToString:@"CloseWebView"])
    {
        [webView removeFromSuperview];
        webView = nil;
    }

}

-(void)closeWebView
{
    if (webView!=nil)
    {
        [webView removeFromSuperview];
        webView = nil;
    }
}

@end

extern "C"
{
    
    void OpenWebViewForIOS(const char *path, const char* gameId, const char* indate, int left, int top, int right, int bottom)
    {
        
        [[BackendWebView instance] openWebView:[NSString stringWithUTF8String:path] gameId:[NSString stringWithUTF8String:gameId] indate:[NSString stringWithUTF8String:indate] left:left top:top right:right bottom:bottom];
    }
    
    void CloseWebViewForIOS()
    {
        [[BackendWebView instance] closeWebView];
    }
}
