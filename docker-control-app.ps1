docker run -it -v $pwd/control-app:/app -w /app -p 3000:3000 node:latest /bin/bash -c /app/start.sh