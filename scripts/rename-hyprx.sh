#!/bin/bash
# Script to replace Hyprx with FrostYeti in all source files and rename files
# This script excludes itself from the replacement to avoid self-modification

set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

SCRIPT_NAME="$(basename "$0")"

echo "Finding files containing 'Hyprx'..."
MATCHES=$(grep -rl "Hyprx" --include="*.cs" --include="*.csproj" --include="*.md" --include="*.json" --include="*.py" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null | grep -v "$SCRIPT_NAME" || true)

if [ -z "$MATCHES" ]; then
    echo "No files containing 'Hyprx' found."
else
    echo ""
    echo "Files to update:"
    echo "$MATCHES"
    echo ""

    # Replace Hyprx with FrostYeti in file contents
    echo "Replacing 'Hyprx' with 'FrostYeti' in file contents..."
    echo "$MATCHES" | while read -r file; do
        if [ -n "$file" ]; then
            sed -i 's/Hyprx/FrostYeti/g' "$file"
            echo "  Updated: $file"
        fi
    done
fi

echo ""
echo "Renaming files containing 'Hyprx' in filename..."
find . -type f -name "*Hyprx*" ! -path "./.git/*" ! -path "*/bin/*" ! -path "*/obj/*" 2>/dev/null | while read -r file; do
    if [ -n "$file" ]; then
        new_name="${file//Hyprx/FrostYeti}"
        if [ "$file" != "$new_name" ]; then
            git mv "$file" "$new_name" 2>/dev/null || mv "$file" "$new_name"
            echo "  Renamed: $file -> $new_name"
        fi
    fi
done

echo ""
echo "Renaming directories containing 'Hyprx' in dirname..."
find . -type d -name "*Hyprx*" ! -path "./.git/*" ! -path "*/bin/*" ! -path "*/obj/*" -depth 2>/dev/null | while read -r dir; do
    if [ -n "$dir" ]; then
        new_name="${dir//Hyprx/FrostYeti}"
        if [ "$dir" != "$new_name" ]; then
            git mv "$dir" "$new_name" 2>/dev/null || mv "$dir" "$new_name"
            echo "  Renamed dir: $dir -> $new_name"
        fi
    fi
done

echo ""
echo "Done! Run 'git diff' to review changes."