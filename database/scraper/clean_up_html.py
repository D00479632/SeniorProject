import os
import re
from bs4 import BeautifulSoup
import glob
import sys

def clean_html(html_content):
    """
    Clean the HTML content by:
    1. Removing <img> tags
    2. Removing <a> tags but keeping their text content
    3. Removing navigation and search links
    4. Trimming content after References or History sections
    """
    # Parse the HTML with BeautifulSoup
    soup = BeautifulSoup(html_content, 'html.parser')
    
    # Remove navigation and search links
    nav_links = soup.select('a.mw-jump-link')
    for link in nav_links:
        link.decompose()
    
    # Remove all img tags
    for img in soup.find_all('img'):
        img.decompose()
    
    # Replace <a> tags with their text content
    for a_tag in soup.find_all('a'):
        # Get the text inside the a tag
        text = a_tag.get_text()
        # Replace the a tag with just its text content
        a_tag.replace_with(text)
    
    # Find and remove content after References or History sections
    pattern = r'<span class="mw-headline" id="(References|History|Notes)"'
    matches = re.search(pattern, str(soup), re.IGNORECASE)
    
    if matches:
        # Convert soup back to string to use regex for removal
        content = str(soup)
        # Find the position of the match
        pos = matches.start()
        # Keep only content before the section
        content = content[:pos]
        # Parse back to BeautifulSoup
        soup = BeautifulSoup(content, 'html.parser')
    
    return str(soup)

def process_single_file(file_path):
    """Process a single HTML file."""
    file_name = os.path.basename(file_path)
    print(f"Processing {file_name}...")
    
    try:
        # Read the file
        with open(file_path, 'r', encoding='utf-8') as file:
            content = file.read()
        
        # Clean the HTML
        cleaned_content = clean_html(content)
        
        # Write the cleaned content back to the file
        with open(file_path, 'w', encoding='utf-8') as file:
            file.write(cleaned_content)
        
        print(f"Successfully cleaned {file_name}")
        return True
    except Exception as e:
        print(f"Error processing {file_name}: {str(e)}")
        return False

def process_all_files(directory):
    """Process all HTML files in the given directory."""
    # Get list of all HTML files
    html_files = glob.glob(os.path.join(directory, "*.html"))
    
    print(f"Found {len(html_files)} HTML files to process")
    
    success_count = 0
    error_count = 0
    
    # Process each file
    for file_path in html_files:
        if process_single_file(file_path):
            success_count += 1
        else:
            error_count += 1
    
    print(f"Cleaning completed: {success_count} files cleaned successfully, {error_count} files with errors")

if __name__ == "__main__":
    # Directory containing HTML files
    html_dir = os.path.join("html/")
    
    # Check if directory exists
    if not os.path.exists(html_dir):
        print(f"Directory {html_dir} does not exist.")
        exit(1)
    
    # Parse command line arguments
    if len(sys.argv) > 1:
        # If a specific file is provided
        if sys.argv[1] == "--all":
            print("Processing all HTML files...")
            process_all_files(html_dir)
        else:
            # Process a specific file
            file_name = sys.argv[1]
            file_path = os.path.join(html_dir, file_name)
            if os.path.exists(file_path):
                process_single_file(file_path)
                print("Single file processing completed.")
            else:
                print(f"File {file_path} does not exist.")
    else:
        # Print usage instructions
        print("Usage:")
        print("  python clean_up_html.py [filename.html]  - Process a single file")
        print("  python clean_up_html.py --all            - Process all HTML files")

# After doing this you can just convert to markdown like this:
# html2markdown --input "html/*.html" --output "md/"