#!/bin/bash
#
# validate-docs.sh - Validates XML documentation comments in C# source files
#
# This script is a wrapper that calls the Python validation script.
# Falls back to a basic shell check if Python is not available.
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Prefer Python script if available
if command -v python3 &> /dev/null; then
    exec python3 "$SCRIPT_DIR/validate-docs.py" "$@"
elif command -v python &> /dev/null; then
    exec python "$SCRIPT_DIR/validate-docs.py" "$@"
else
    echo "WARNING: Python not found, using basic shell validation"
    echo "For full validation, please install Python 3"
    echo ""
    
    # Basic shell fallback - just check for obvious issues
    REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
    ERRORS=0
    
    # Check for placeholder patterns
    while IFS= read -r file; do
        # Check for "invoke the member here" placeholder
        if grep -q "invoke the member here" "$file" 2>/dev/null; then
            echo "ERROR: $file contains placeholder 'invoke the member here'"
            ERRORS=$((ERRORS + 1))
        fi
        # Check for "invoke member here" placeholder
        if grep -q "invoke member here" "$file" 2>/dev/null; then
            echo "ERROR: $file contains placeholder 'invoke member here'"
            ERRORS=$((ERRORS + 1))
        fi
    done < <(find "$REPO_ROOT" -path "*/src/*.cs" -o -path "*/src/**/*.cs" 2>/dev/null | grep -v "/bin/" | grep -v "/obj/")
    
    if [[ $ERRORS -gt 0 ]]; then
        echo ""
        echo "VALIDATION FAILED: $ERRORS placeholder(s) found"
        exit 1
    fi
    
    echo "VALIDATION PASSED: Basic check completed"
    exit 0
fi