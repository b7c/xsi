#load "../xsi-extension.csx"

using Xabbo.Messages;

Dictionary<Header, List<Action<InterceptArgs>>> __intercepts = new();

// Registers an intercept for the specified header
void Intercept(Header header, Action<InterceptArgs> callback) {
    __xtn.Dispatcher.AddIntercept(header, callback, __xtn.ClientType);
    if (!__intercepts.TryGetValue(header, out var callbackList)) {
        callbackList = new List<Action<InterceptArgs>>();
        __intercepts.Add(header, callbackList);
    }
    callbackList.Add(callback);
}

// Registers an intercept for the specified headers
void Intercept(Header[] headers, Action<InterceptArgs> callback) {
    foreach (var header in headers) {
        Intercept(header, callback);
    }
}

// Registers an async intercept for the specified header
void Intercept(Header header, Func<InterceptArgs, Task> callback) {
    Intercept(header, e => { callback(e); });
}

// Registers an async intercept for the specified headers
void Intercept(Header[] headers, Func<InterceptArgs, Task> callback) {
    foreach (var header in headers) {
        Intercept(header, callback);
    }
}

// Clears intercepts for the specified headers
void ClearIntercepts(params Header[] headers) {
    foreach (var header in headers) {
        if (__intercepts.TryGetValue(header, out var callbackList)) {
            foreach (var callback in callbackList) {
                __xtn.Dispatcher.RemoveIntercept(header, callback);
            }
            callbackList.Clear();
        }
    }
}

// Clears all intercepts
void ClearIntercepts() {
    foreach (var (header, list) in __intercepts) {
        foreach (var callback in list) {
            __xtn.Dispatcher.RemoveIntercept(header, callback);
        }
        list.Clear();
    }
}