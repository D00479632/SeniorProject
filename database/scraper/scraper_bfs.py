from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
import os
import re
from urllib.parse import urlparse
from selenium.common.exceptions import StaleElementReferenceException
import time

class StarDewScraper:
    def __init__(self):
        self.driver = webdriver.Firefox()
        self.links = set()  # Set to store links to visit
        self.visited = set()  # Set to store already visited links
    
    def start_scraper(self):
        self.driver.get("https://www.stardewvalleywiki.com/Stardew_Valley_Wiki")
        table = self.driver.find_element(By.CSS_SELECTOR, "#mainmenucontainer table")
        # Get all the href links
        anchors = table.find_elements(By.TAG_NAME, "a")
        print(f"Found {len(anchors)} anchors in the table")
        for anchor in anchors:
            href = anchor.get_attribute("href")
            if href and href.startswith('https://www.stardewvalleywiki.com/'):
                self.links.add(href)
        print(f"Initial links found: {len(self.links)}")
    
    def normalize_url(self, url):
        """Ensure all URLs use the www.stardewvalleywiki.com version."""
        return url.replace("https://www.stardewvalleywiki.com", "https://stardewvalleywiki.com")
    
   def scrape_page(self, url):
    """Process a single page, extract content and find new links"""
    if url in self.visited:
        return

    # Skip URLs with specific prefixes
    skip_prefixes = ['Special:', 'Modding:', 'Talk:', 'File:', 'Category:']
    if any(prefix in url for prefix in skip_prefixes):
        print(f"Skipping URL with unwanted prefix: {url}")
        return

    try:
        print(f"Visiting: {url}")
        self.driver.get(url)
        self.visited.add(url)  # Mark as visited
        
        # Extract and save content
        self.write_stuff()
        
        # Find all links on the page
        new_links = self.driver.find_elements(By.TAG_NAME, 'a')
        for new_link in new_links:
            try:
                href = new_link.get_attribute('href')
                if href and href.startswith('https://www.stardewvalleywiki.com/') and href not in self.visited and href not in self.links:
                    # Filter out non-article links
                    if '#' in href or 'png' in href or 'file:' in href or 'php' in href:
                        continue
                    # Skip URLs with specific prefixes
                    if any(prefix in href for prefix in skip_prefixes):
                        continue
                    self.links.add(href)
            except StaleElementReferenceException:
                continue  # Skip stale elements
    except Exception as e:
        print(f"Error processing {url}: {e}") 

    def write_stuff(self):
        try:
            header = self.driver.find_element(By.ID, "firstHeading").text.strip()
            text_body = self.driver.find_element(By.ID, "bodyContent").text
            
            # Get the current URL
            url = self.driver.current_url.lower()
            
            # Skip non-article pages
            if any(x in url for x in ['#', 'png', 'file:', 'php']):
                return
                
            # Remove spaces and special characters from the filename
            safe_header = re.sub(r'\W+', '_', header)
            
            # Define the base directory
            save_dir = "filestore/"
            os.makedirs(save_dir, exist_ok=True)
            
            # Generate the filename
            filename = f"{save_dir}{safe_header}.txt"
            
            # If the file exists, skip writing
            if os.path.exists(filename):
                print(f"File '{filename}' already exists. Skipping...")
                return
                
            # Write the content to the file
            with open(filename, "w", encoding="utf-8") as file:
                file.write(text_body)
            print(f"Saved to: {filename}")
        except Exception as e:
            print(f"Error writing content: {e}")
    
    def __exit__(self):
        self.driver.quit()

def main():
    s = StarDewScraper()
    s.start_scraper()
    
    # Breadth-first scraping
    iteration = 1
    while s.links:
        print(f"\nIteration {iteration} - Links to process: {len(s.links)}")
        # Convert the set to a list to avoid modifying during iteration
        current_links = list(s.links)
        s.links.clear()  # Clear for the next level
        
        for link in current_links:
            s.scrape_page(link)
            
        print(f"Iteration {iteration} complete. New links found: {len(s.links)}")
        print(f"Total pages visited: {len(s.visited)}")
        iteration += 1
    
    print("Scraping complete!")
    s.__exit__()

if __name__ == '__main__':
    main()
