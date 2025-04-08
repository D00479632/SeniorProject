#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "Checking services status..."

# Check if Ollama is running
if pgrep -x "ollama" > /dev/null; then
    echo -e "${GREEN}✓ Ollama is running${NC}"
    
    # Check if Gemma 3B model is available
    if ollama list | grep -q "gemma3:4b"; then
        echo -e "${GREEN}✓ Gemma 3B model is available${NC}"
    else
        echo -e "${YELLOW}! Gemma 3B model is not available${NC}"
        echo "To pull the Gemma 3B model, run: ollama pull gemma:3b"
    fi
else
    echo -e "${RED}✗ Ollama is not running${NC}"
    echo "To start Ollama, run: ollama serve"
fi

# Check if Marqo is running
if curl -s http://localhost:8882/health > /dev/null; then
    echo -e "${GREEN}✓ Marqo is running${NC}"
else
    echo -e "${RED}✗ Marqo is not running${NC}"
    echo "To start Marqo, run: docker run -p 8882:8882 marqoai/marqo:latest"
fi 