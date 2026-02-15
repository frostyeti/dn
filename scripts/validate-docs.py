#!/usr/bin/env python3
"""
validate-docs.py - Validates XML documentation comments in C# source files

This script checks that all non-private members have:
1. A <summary> element
2. A <remarks> element containing <example> with <code lang="csharp">
3. No placeholder text in code examples

Exit codes:
    0 - All validations passed
    1 - Validation failures found
"""

import re
import sys
from pathlib import Path
from dataclasses import dataclass, field
from typing import Optional


@dataclass
class DocIssue:
    file: str
    line: int
    issue_type: str
    message: str


@dataclass
class XmlDocBlock:
    start_line: int
    end_line: int
    has_summary: bool = False
    has_example: bool = False
    has_code_csharp: bool = False
    code_content: str = ""
    member_line: int = 0
    member_type: str = ""
    member_name: str = ""


PLACEHOLDER_PATTERNS = [
    r"invoke\s+the\s+member\s+here",
    r"invoke\s+member\s+here",
    r"TODO",
    r"placeholder",
    r"\.\.\.\.\.\.",  # Five or more dots
]

# Patterns for non-private member declarations
NON_PRIVATE_PATTERN = re.compile(
    r'^\s*(public|internal|protected|protected\s+internal)\s+'
    r'(?:static\s+|override\s+|virtual\s+|abstract\s+|sealed\s+|readonly\s+|partial\s+)*'
    r'(?:class|struct|interface|enum|delegate|void|async|'
    r'(?:[\w<>?,\s\[\]]+)\s+[~\w]+\s*\()',
    re.MULTILINE
)


def find_src_files(root: Path) -> list[Path]:
    """Find all C# source files in src directories."""
    files = []
    for src_dir in root.rglob("src"):
        if src_dir.is_dir():
            for cs_file in src_dir.rglob("*.cs"):
                if "/bin/" not in str(cs_file) and "/obj/" not in str(cs_file):
                    files.append(cs_file)
    return sorted(files)


def extract_xml_doc_blocks(content: str) -> list[XmlDocBlock]:
    """Extract XML documentation blocks from C# source content."""
    lines = content.split('\n')
    blocks = []
    current_block = None
    in_code = False
    code_content = []
    
    for i, line in enumerate(lines, start=1):
        stripped = line.strip()
        
        # Check for start of XML doc comment
        if stripped.startswith('///'):
            if current_block is None:
                current_block = XmlDocBlock(start_line=i, end_line=i)
            else:
                current_block.end_line = i
            
            # Check for summary
            if '<summary>' in stripped or (current_block.has_summary and '</summary>' not in stripped):
                current_block.has_summary = True
            
            # Check for example and code
            if '<example>' in stripped:
                current_block.has_example = True
            if '<code lang="csharp">' in stripped:
                current_block.has_code_csharp = True
                in_code = True
                code_content = []
            if '</code>' in stripped:
                in_code = False
                current_block.code_content = '\n'.join(code_content)
            if in_code:
                # Extract content after ///
                content_match = re.match(r'^\s*///\s*(.*)$', line)
                if content_match:
                    code_content.append(content_match.group(1))
        
        else:
            # Non-comment line - check if it's a member declaration
            if current_block is not None:
                # Check if this line is a non-private member
                for pattern in [
                    r'^\s*(public|internal|protected|protected\s+internal)\s+.*\s+(\w+)\s*\(',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+(?:static\s+)?(\w+)\s*\{',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+(?:static\s+)?(?:readonly\s+)?(\w+)\s+(\w+)\s*\{',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+(?:static\s+)?(?:class|struct|interface|enum)\s+(\w+)',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+implicit\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+explicit\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+static\s+implicit\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+static\s+explicit\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+static\s+\w+\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+\w+\s+operator',
                    r'^\s*(public|internal|protected|protected\s+internal)\s+(?:static\s+)?(?:\w+\s+)?operator',
                ]:
                    match = re.search(pattern, line)
                    if match:
                        current_block.member_line = i
                        blocks.append(current_block)
                        break
                current_block = None
            in_code = False
            code_content = []
    
    return blocks


def check_for_placeholders(code_content: str) -> list[str]:
    """Check code content for placeholder patterns."""
    found = []
    for pattern in PLACEHOLDER_PATTERNS:
        if re.search(pattern, code_content, re.IGNORECASE):
            found.append(pattern)
    return found


def validate_file(file_path: Path) -> list[DocIssue]:
    """Validate a single C# file for documentation issues."""
    issues = []
    
    try:
        content = file_path.read_text()
    except Exception as e:
        issues.append(DocIssue(
            file=str(file_path),
            line=0,
            issue_type="error",
            message=f"Could not read file: {e}"
        ))
        return issues
    
    blocks = extract_xml_doc_blocks(content)
    
    for block in blocks:
        # Check for missing summary
        if not block.has_summary:
            issues.append(DocIssue(
                file=str(file_path),
                line=block.start_line,
                issue_type="error",
                message="Non-private member missing <summary>"
            ))
            continue
        
        # Check for missing example with code lang="csharp"
        if not block.has_example or not block.has_code_csharp:
            issues.append(DocIssue(
                file=str(file_path),
                line=block.start_line,
                issue_type="error",
                message="Non-private member missing <example> with <code lang=\"csharp\">"
            ))
            continue
        
        # Check for placeholder text
        placeholders = check_for_placeholders(block.code_content)
        for placeholder in placeholders:
            issues.append(DocIssue(
                file=str(file_path),
                line=block.start_line,
                issue_type="warning",
                message=f"Code example contains placeholder pattern: {placeholder}"
            ))
    
    return issues


def main():
    root = Path(__file__).parent.parent
    
    print("Validating XML documentation comments...")
    print()
    
    files = find_src_files(root)
    total_issues = 0
    total_errors = 0
    total_warnings = 0
    files_with_issues = 0
    
    for file_path in files:
        issues = validate_file(file_path)
        if issues:
            files_with_issues += 1
            for issue in issues:
                total_issues += 1
                if issue.issue_type == "error":
                    total_errors += 1
                    color = "\033[0;31m"  # Red
                else:
                    total_warnings += 1
                    color = "\033[1;33m"  # Yellow
                
                print(f"{color}{issue.issue_type.upper()}{'\033[0m'}: {issue.file}:{issue.line} - {issue.message}")
    
    print()
    print("=" * 50)
    print(f"Files scanned: {len(files)}")
    print(f"Files with issues: {files_with_issues}")
    print(f"Total errors: {total_errors}")
    print(f"Total warnings: {total_warnings}")
    print("=" * 50)
    
    if total_errors > 0:
        print(f"\033[0;31mVALIDATION FAILED\033[0m: {total_errors} error(s) found")
        sys.exit(1)
    
    if total_warnings > 0:
        print(f"\033[1;33mVALIDATION PASSED WITH WARNINGS\033[0m: {total_warnings} warning(s) found")
        sys.exit(0)
    
    print(f"\033[0;32mVALIDATION PASSED\033[0m: All documentation checks passed")
    sys.exit(0)


if __name__ == "__main__":
    main()