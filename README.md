== AutoLock

Want to remotely / automatically lock your Windows screen? AutoLock exposes a REST endpoint to trigger a screen lock (from your phone, from home automation systems, etc.).

Simply launch the application at user login (add a shortcut to the `Startup` menu items) and then invoke:

`http://localhost:4352/lock`

You can also launch the application with:

```bash
 --port,-p  Change the default port (4352)
 --token,-t Set a token that needs to be passed via query string to the URL for authentication
 ```

== Downloads

https://github.com/pierotofy/AutoLock/releases

== Contributions

Contributions welcome! Just open a PR.

== License

GPL 3.0