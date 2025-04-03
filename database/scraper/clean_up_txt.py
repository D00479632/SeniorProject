import os
import re

def clean_file(file_path):
    """
    Cleans a file by:
    1. Removing "Jump to navigation" and "Jump to search" lines
    2. Removing content starting from lines beginning with 'History' or 'References'
    
    Args:
        file_path (str): Path to the file to clean
    
    Returns:
        bool: True if file was modified, False otherwise
    """
    print(f"Processing file: {file_path}")
    
    # Read the file content
    with open(file_path, 'r', encoding='utf-8') as file:
        lines = file.readlines()
    
    # Find the first occurrence of History or References at the beginning of a line
    cut_index = None
    for i, line in enumerate(lines):
        if re.match(r'^History', line) or re.match(r'^References', line):
            cut_index = i
            print(f"Found marker '{line.strip()}' at line {i+1}")
            break
    
    # If we found a marker, truncate the file
    if cut_index is not None:
        # Keep only the lines before the marker
        lines = lines[:cut_index]
    
    # Remove "Jump to navigation" and "Jump to search" lines
    original_line_count = len(lines)
    lines = [line for line in lines if not line.strip() == "Jump to navigation" and not line.strip() == "Jump to search"]
    
    jump_lines_removed = original_line_count - len(lines)
    if jump_lines_removed > 0:
        print(f"Removed {jump_lines_removed} 'Jump to' lines")
    
    # Write the cleaned content back to the file
    with open(file_path, 'w', encoding='utf-8') as file:
        file.writelines(lines)
    
    # If we either truncated the file or removed Jump lines, report modification
    was_modified = (cut_index is not None) or (jump_lines_removed > 0)
    if was_modified:
        print(f"File cleaned. Final line count: {len(lines)}")
    else:
        print("No modifications needed for this file")
    
    return was_modified

def clean_all_files(directory):
    """
    Cleans all .txt files in the given directory.
    
    Args:
        directory (str): Path to the directory containing text files
    
    Returns:
        tuple: (files_processed, files_modified) counts
    """
    files_processed = 0
    files_modified = 0
    
    # Get all .txt files in the directory
    txt_files = [f for f in os.listdir(directory) if f.endswith('.txt')]
    total_files = len(txt_files)
    
    print(f"Found {total_files} text files to process")
    
    for i, filename in enumerate(txt_files, 1):
        file_path = os.path.join(directory, filename)
        print(f"\n[{i}/{total_files}] Processing {filename}")
        
        try:
            modified = clean_file(file_path)
            files_processed += 1
            if modified:
                files_modified += 1
        except Exception as e:
            print(f"Error processing {filename}: {str(e)}")
    
    return files_processed, files_modified

# TODO: Maybe I will get ridd of files that aren't a certain length

# Process all files in the directory
if __name__ == "__main__":
    # Path to the directory containing text files
    txt_dir = 'txt/'
    
    print(f"Starting cleanup process for all files in {txt_dir}")
    processed, modified = clean_all_files(txt_dir)
    
    print(f"\nCleanup complete!")
    print(f"Files processed: {processed}")
    print(f"Files modified: {modified}")
