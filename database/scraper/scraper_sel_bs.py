from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
import os
import re
from urllib.parse import urlparse
from selenium.common.exceptions import StaleElementReferenceException
import time
from bs4 import BeautifulSoup

class StarDewScraper:
    def __init__(self):
        self.driver = webdriver.Firefox()
        self.links = set()  # Set to store links to visit
        self.visited = set()  # Set to store already visited links
    
    def is_valid_url(self, url):
        """
        Check if the URL is valid for scraping
        Exclude Special, Modding, Talk, File, and Category pages
        """
        # List of URL segments to exclude
        exclude_segments = [
            '/Special:', 
            '/Modding:', 
            '/Talk:', 
            '/File:', 
            '/Category:',
            '/User:',
            '/Template_talk:',
            '/User_talk:',
            '/Module:',
            '/Template:'
        ]
        
        # Check if any excluded segment is in the URL
        if any(segment in url for segment in exclude_segments):
            return False
        
        # Additional filters from previous implementation
        if ('#' in url or 
            'png' in url or 
            'file:' in url or 
            'php' in url):
            return False
        
        # Ensure it's a Stardew Valley Wiki page
        return url.startswith('https://www.stardewvalleywiki.com/')
    
    def start_scraper(self):
        self.driver.get("https://www.stardewvalleywiki.com/Stardew_Valley_Wiki")
        table = self.driver.find_element(By.CSS_SELECTOR, "#mainmenucontainer table")
        # Get all the href links
        anchors = table.find_elements(By.TAG_NAME, "a")
        print(f"Found {len(anchors)} anchors in the table")
        for anchor in anchors:
            href = anchor.get_attribute("href")
            if href and self.is_valid_url(href):
                self.links.add(href)
        print(f"Initial links found: {len(self.links)}")
    
    def scrape_page(self, url):
        """Process a single page, extract content and find new links"""
        if url in self.visited:
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
                    if href and self.is_valid_url(href) and href not in self.visited:
                        self.links.add(href)
                except StaleElementReferenceException:
                    continue  # Skip stale elements
        except Exception as e:
            print(f"Error processing {url}: {e}")
    
    def write_stuff(self):
        try:
            # Use BeautifulSoup to parse the page source
            soup = BeautifulSoup(self.driver.page_source, 'html.parser')
            
            # Find the page header and first heading
            header = soup.find(id="firstHeading")
            header_text = header.text.strip() if header else "Untitled"
            
            # Find the main content body
            body_content = soup.find(id="bodyContent")
            
            # Skip if no body content found
            if not body_content:
                return
                
            # Remove spaces and special characters from the filename
            safe_header = re.sub(r'\W+', '_', header_text)
            
            # Define the base directory
            save_dir = "files/"
            os.makedirs(save_dir, exist_ok=True)
            
            # Generate the filename
            filename = f"{save_dir}{safe_header}.html"
            
            # If the file exists, skip writing
            if os.path.exists(filename):
                print(f"File '{filename}' already exists. Skipping...")
                return
            
            # Write the entire body content as HTML
            with open(filename, "w", encoding="utf-8") as file:
                file.write(str(body_content))
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
