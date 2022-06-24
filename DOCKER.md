## Docker

- To build Docker image, first make sure to run buid ClientApp because right now, we are not building ClientApp in DockerFile
  ```
  cd ClientApp/
  npm run build
  ```
- Build docker image
  ```
  docker build -t horizech/apiraiser .
  ```
- Add tags to docker image
  ```
  docker image tag horizech/apiraiser horizech/apiraiser:latest
  docker image tag horizech/apiraiser horizech/apiraiser:0.2.0
  ```
- Push docker image with all tags to docker hub
  ```
  docker push horizech/apiraiser -a
  ```
- Set up environment variables by using .env file or mentioning in Dockerfile
- Run docker container using docker-compose
  ```
  docker-compose up -d
  ```
