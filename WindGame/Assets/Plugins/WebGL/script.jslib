var MyPlugin = {
    Hello: function()
    {
        anotherFunction();
    },
    HelloString: function(str)
    {
        window.alert(Pointer_stringify(str));
    }
};

mergeInto(LibraryManager.library, MyPlugin);