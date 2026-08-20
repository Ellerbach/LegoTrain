#!/bin/bash
#
# Builds the LegoTrain ASP.NET Docker image.
#
# Usage:
#   ./build-docker.sh [OPTIONS]
#
# Options:
#   -p, --platform PLATFORM    Target platform architecture (amd64, arm32, or arm64). Default: amd64
#   -v, --version VERSION      Version tag for the image. Default: 1.0
#   -r, --registry REGISTRY    Docker registry/repository name. Default: ellerbach/legotrain
#   --push                     Push the image to registry after building
#   -h, --help                 Display this help message
#
# Examples:
#   ./build-docker.sh --platform arm32 --version 1.0
#   ./build-docker.sh -p arm64 -v 2.3 --push
#   ./build-docker.sh -p arm64 -v 2.3 -r myregistry/legotrain --push
#

set -e  # Exit on error

# Default values
PLATFORM="amd64"
VERSION="1.0"
REGISTRY="ellerbach/legotrain"
PUSH_IMAGE="no"

# Function to display help
show_help() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -p, --platform PLATFORM    Target platform architecture (amd64, arm32, or arm64). Default: amd64"
    echo "  -v, --version VERSION      Version tag for the image. Default: 1.0"
    echo "  -r, --registry REGISTRY    Docker registry/repository name. Default: ellerbach/legotrain"
    echo "  --push                     Push the image to registry after building"
    echo "  -h, --help                 Display this help message"
    echo ""
    echo "Examples:"
    echo "  $0 --platform arm32 --version 1.0"
    echo "  $0 -p arm64 -v 2.3 --push"
    echo "  $0 -p arm64 -v 2.3 -r myregistry/legotrain --push"
    exit 0
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -p|--platform)
            PLATFORM="$2"
            shift 2
            ;;
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        -r|--registry)
            REGISTRY="$2"
            shift 2
            ;;
        --push)
            PUSH_IMAGE="yes"
            shift
            ;;
        -h|--help)
            show_help
            ;;
        *)
            echo "Error: Unknown option: $1" >&2
            echo "Use --help for usage information" >&2
            exit 1
            ;;
    esac
done

# Validate platform
if [[ "$PLATFORM" != "amd64" && "$PLATFORM" != "arm32" && "$PLATFORM" != "arm64" ]]; then
    echo "Error: Platform must be 'amd64', 'arm32', or 'arm64'" >&2
    echo "Usage: $0 [platform] [version] [registry]" >&2
    exit 1
fi

# Determine which Dockerfile to use
case "$PLATFORM" in
    amd64)
        DOCKERFILE_PATH="LegoTrain/Dockerfile"
        ;;
    arm32)
        DOCKERFILE_PATH="LegoTrain/Dockerfile.arm32"
        ;;
    arm64)
        DOCKERFILE_PATH="LegoTrain/Dockerfile.arm64"
        ;;
esac

# Build the image tag
IMAGE_TAG="${REGISTRY}:${PLATFORM}-${VERSION}"

echo "Building LegoTrain Docker image..."
echo "  Platform:    $PLATFORM"
echo "  Version:     $VERSION"
echo "  Dockerfile:  $DOCKERFILE_PATH"
echo "  Image Tag:   $IMAGE_TAG"
echo ""

# Build the Docker image
if ! docker build -f "$DOCKERFILE_PATH" -t "$IMAGE_TAG" .; then
    echo ""
    echo "✗ Build failed"
    exit 1
fi

echo ""
echo "✓ Build completed successfully!"
echo "Image: $IMAGE_TAG"

# Push if requested
if [[ "$PUSH_IMAGE" == "push" || "$PUSH_IMAGE" == "yes" || "$PUSH_IMAGE" == "true" ]]; then
    echo ""
    echo "Preparing to push image to registry..."
    
    # Extract registry hostname
    # If registry has a slash, check if the part before slash is a domain (contains a dot)
    # Otherwise default to docker.io for Docker Hub repositories
    if [[ "$REGISTRY" =~ ^([^/]+)/ ]]; then
        REGISTRY_PREFIX="${BASH_REMATCH[1]}"
        # Check if prefix contains a dot (indicating it's a domain name)
        if [[ "$REGISTRY_PREFIX" == *.* ]]; then
            REGISTRY_HOST="$REGISTRY_PREFIX"
        else
            REGISTRY_HOST="docker.io"
        fi
    else
        REGISTRY_HOST="docker.io"
    fi
    
    # Check if already logged in
    echo "Checking authentication for registry: $REGISTRY_HOST"
    
    CONFIG_PATH="$HOME/.docker/config.json"
    NEEDS_LOGIN=true
    
    if [ -f "$CONFIG_PATH" ]; then
        if grep -q "$REGISTRY_HOST" "$CONFIG_PATH" 2>/dev/null; then
            NEEDS_LOGIN=false
            echo "Already authenticated to $REGISTRY_HOST"
        fi
    fi
    
    # Prompt for login if needed
    if [ "$NEEDS_LOGIN" = true ]; then
        echo ""
        echo "Not authenticated to $REGISTRY_HOST"
        echo "Please log in to the registry:"
        
        if ! docker login "$REGISTRY_HOST"; then
            echo ""
            echo "✗ Login failed. Image will not be pushed."
            exit 1
        fi
    fi
    
    # Push the image
    echo ""
    echo "Pushing image: $IMAGE_TAG"
    if docker push "$IMAGE_TAG"; then
        echo ""
        echo "✓ Image pushed successfully!"
    else
        echo ""
        echo "✗ Push failed"
        exit 1
    fi
else
    echo ""
    echo "To push the image, run:"
    echo "  docker push $IMAGE_TAG"
    echo "Or re-run with 'push' as the fourth argument"
fi

exit 0
