#!/bin/bash

IMAGE_REPOSITORY="jarbounds"
IMAGE_NAME="runtestsworkerservice"
IMAGE_VERSION="1.0.0"

sudo docker buildx build --push --tag "$IMAGE_REPOSITORY/$IMAGE_NAME:latest" --tag "$IMAGE_REPOSITORY/$IMAGE_NAME:$IMAGE_VERSION" --platform linux/amd64,linux/arm64 -f Dockerfile .
