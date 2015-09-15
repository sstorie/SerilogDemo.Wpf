## SerilogDemo.Wpf
This repository contains an example WPF application that uses Serilog for logging. It was created to explore potential issues with using Serilog in this context of a WPF app.

This application uses Autofac to inject the logger into the MainWindow class, which provides the *SourceContext* value to log events. Other values are added to enrich log statements, but there is a problem when enrichment is done on different threads.

### Issue #1 - Using LogContext.PushProperty with async/await
One issue encountered is the values pushed into the LogContext property after an async call returns are lost when the method completes. From the documentation it's clear you can push values into the context, and they'll persist even if that thread is interrupted using async/await. However, if you push a value *after* a task is await'd, that value will not persist back to the calling thread.

For example, if we look at the code in [MainWindow.xaml.cs](https://github.com/sstorie/SerilogDemo.Wpf/blob/develop/SerilogDemo.Wpf/SerilogDemo.Wpf/MainWindow.xaml.cs) we see it simulates an async login process where we may want to set the value of an employee ID after the login is successful. However, we can see that the context value is passed into threads created with async/await, but is lost when set after an await happens.

The last line shows a case where we've pushed a value into the context, but is lost because it was not on the original thread. Specifically, setting the *ContextValue* to 4 on thread ID 13 is lost when control returns back to thread ID 10.

	2015-09-15 09:02:14.443 -05:00|Level:Debug|ThreadId:10|ContextValue:1|SourceContext:|Message:Checking context value before mainWindow.ShowDialog()
	2015-09-15 09:02:14.620 -05:00|Level:Debug|ThreadId:10|ContextValue:2|SourceContext:SerilogDemo.Wpf.MainWindow|Message:MainWindow initialized - context value should be 2
	2015-09-15 09:02:17.437 -05:00|Level:Debug|ThreadId:10|ContextValue:2|SourceContext:SerilogDemo.Wpf.MainWindow|Message:Log event
	2015-09-15 09:02:20.036 -05:00|Level:Debug|ThreadId:10|ContextValue:2|SourceContext:SerilogDemo.Wpf.MainWindow|Message:Before logging user in - context value should be 2
	2015-09-15 09:02:20.039 -05:00|Level:Debug|ThreadId:14|ContextValue:3|SourceContext:SerilogDemo.Wpf.MainWindow|Message:SimulateLogin start - context value should be 3
	2015-09-15 09:02:20.039 -05:00|Level:Debug|ThreadId:13|ContextValue:3|SourceContext:SerilogDemo.Wpf.MainWindow|Message:on login background thread... - context value should be 3
	2015-09-15 09:02:22.040 -05:00|Level:Debug|ThreadId:13|ContextValue:4|SourceContext:SerilogDemo.Wpf.MainWindow|Message:SimulateLogin end - context value should be 4
	2015-09-15 09:02:22.042 -05:00|Level:Debug|ThreadId:10|ContextValue:2|SourceContext:SerilogDemo.Wpf.MainWindow|Message:after logging user in - context value should be 4 (but is 2!)
