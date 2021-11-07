﻿using System;

namespace Email.NET.Test
{
    public static class MockData
    {
        public const string TestFileBase64Value = "TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2NpbmcgZWxpdC4gTWFlY2VuYXMgb3JuYXJlIGVmZmljaXR1ciBtaSBxdWlzIGxhb3JlZXQuIE1vcmJpIHZvbHV0cGF0IHNlZCBlcm9zIG5vbiBjdXJzdXMuIERvbmVjIHVsbGFtY29ycGVyIGV4IGV1IGxpZ3VsYSBjdXJzdXMgdmVoaWN1bGEuIEZ1c2NlIG5lcXVlIGxlY3R1cywgaW1wZXJkaWV0IG1heGltdXMgb3JuYXJlIG5vbiwgdWxsYW1jb3JwZXIgc2l0IGFtZXQgZHVpLiBNb3JiaSBldCBzb2RhbGVzIGRpYW0sIHNpdCBhbWV0IHBvcnRhIGxvcmVtLiBDdXJhYml0dXIgcGVsbGVudGVzcXVlLCB2ZWxpdCB2ZWwgdmVzdGlidWx1bSByaG9uY3VzLCBuaXNpIGFudGUgcG9zdWVyZSBmZWxpcywgcXVpcyBhbGlxdWV0IGVyYXQgbGFjdXMgYWMgbWF1cmlzLiBDcmFzIG5vbiBhY2N1bXNhbiBuZXF1ZS4gRG9uZWMgYW50ZSB2ZWxpdCwgdGVtcHVzIGF0IHZ1bHB1dGF0ZSBldCwgc2VtcGVyIGlkIG51bGxhLiBOdW5jIGFjIG1pIHNlZCBzZW0gcGVsbGVudGVzcXVlIHNvbGxpY2l0dWRpbiBub24gYWMgbnVsbGEuIE51bGxhbSBmZXJtZW50dW0gZXJhdCBldCBuaXNpIHRpbmNpZHVudCB2dWxwdXRhdGUuIE51bGxhIGZldWdpYXQgcXVhbSBldCBsZWN0dXMgcGxhY2VyYXQsIGEgcmhvbmN1cyB1cm5hIGZldWdpYXQuIEludGVnZXIgdXQgZXggdHVycGlzLiBTZWQgZWxlbWVudHVtIGxlbyBhdCBsZW8gZWxlaWZlbmQgdHJpc3RpcXVlLiBOdW5jIHBvc3VlcmUgdWx0cmljaWVzIHNhcGllbiBxdWlzIG1vbGxpcy4gRXRpYW0gY29udmFsbGlzIGludGVyZHVtIG1vbGVzdGllLiBBbGlxdWFtIHNpdCBhbWV0IHRvcnRvciBlbmltLg==";

        internal static byte[] TestFileAsByteArray() => Convert.FromBase64String(TestFileBase64Value);
    }
}
