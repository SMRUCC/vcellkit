The project output has been configured at ``./bin/`` directory, once done the git clone of this repository, just open the solution file [WebApp.sln](../WebApp.sln), and config to release mode, and compile the solution.
And start the demo by running this script:

```bash
#!/bin/bash

# start httpd server with specific web root directory ../wwwroot/
# using default port: 80
# using default threadpool
./bin/httpd /start /wwwroot ../wwwroot/
```
