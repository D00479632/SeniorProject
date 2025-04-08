from marqo import Client
import os
import json
import time

# Define paths and settings
filestore_path = 'txt/'
index_name = 'stardew-valley-data'
tracking_file = 'indexed_files.json'
batch_size = 15  # Number of files to process in each run
cooldown_seconds = 10  # Time to wait between batches

# Set up Marqo Client
mq = Client(url='http://localhost:8882')

def load_tracking_data():
    """Load information about which files have already been indexed"""
    if os.path.exists(tracking_file):
        with open(tracking_file, 'r') as f:
            return json.load(f)
    return {"indexed_files": [], "index_exists": False}

def save_tracking_data(tracking_data):
    """Save information about which files have been indexed"""
    with open(tracking_file, 'w') as f:
        json.dump(tracking_data, f)

def create_index_if_needed(tracking_data):
    """Create the index if it doesn't already exist"""
    if not tracking_data["index_exists"]:
        try:
            # Only delete if it exists but we think it doesn't
            mq.index(index_name).delete()
        except:
            pass
        
        mq.create_index(index_name)
        tracking_data["index_exists"] = True
        save_tracking_data(tracking_data)
        print(f"Created new index: {index_name}")
    else:
        print(f"Using existing index: {index_name}")

def get_next_batch(tracking_data):
    """Get the next batch of files to process"""
    indexed_files = set(tracking_data["indexed_files"])
    
    # Get list of all files that haven't been indexed yet
    pending_files = []
    for filename in os.listdir(filestore_path):
        if filename.endswith('.txt') and filename not in indexed_files:
            pending_files.append(filename)
    
    # Return a batch of files (up to batch_size)
    return pending_files[:batch_size]

def index_batch(batch_files, tracking_data):
    """Index a batch of files"""
    if not batch_files:
        print("No files to index. All files have been processed!")
        return False
    
    print(f"Processing batch of {len(batch_files)} files: {', '.join(batch_files)}")
    
    # Load the files
    documents = []
    for filename in batch_files:
        with open(os.path.join(filestore_path, filename), 'r') as file:
            filecontent = file.read()
            documents.append({
                'Title': filename, 
                'Description': filecontent
            })
    
    # Index the documents
    try:
        print(f"Adding {len(documents)} documents to index...")
        mq.index(index_name).add_documents(
            documents,
            tensor_fields=["Title", "Description"]
        )
        
        # Update tracking data
        tracking_data["indexed_files"].extend(batch_files)
        save_tracking_data(tracking_data)
        
        print(f"Successfully indexed {len(batch_files)} files.")
        print(f"Total files indexed so far: {len(tracking_data['indexed_files'])}")
        return True
        
    except Exception as e:
        print(f"Error indexing batch: {str(e)}")
        return False

def run_test_search():
    """Run a test search to verify the index is working"""
    question = "What gifts does Clint love?"
    print(f"\nRunning test search: '{question}'")
    
    try:
        results = mq.index(index_name).search(
            q=question,
            limit=2
        )
        
        context = ''
        for i, hit in enumerate(results['hits']):
            title = hit['Title']
            text = hit['Description']
            context += f"Source {i + 1}) {title} || {text} \n"
        
        print("Search Results:", context)
    except Exception as e:
        print(f"Error during test search: {str(e)}")

def process_all_files():
    """Process all files in batches with cooldown periods"""
    print("Starting Marqo indexing process for all files")
    
    # Load tracking data
    tracking_data = load_tracking_data()
    
    # Create index if needed
    create_index_if_needed(tracking_data)
    
    # Get total number of files for progress tracking
    total_files = len([f for f in os.listdir(filestore_path) if f.endswith('.txt')])
    indexed_count = len(tracking_data["indexed_files"])
    
    print(f"Starting indexing process: {indexed_count}/{total_files} files already indexed")
    
    # Check if all files are already indexed
    if indexed_count >= total_files:
        print("All files are already indexed.")
        run_test_search()
        return
    
    # Process files in batches until all are done
    batch_count = 0
    all_indexed = False
    
    while True:
        # Get next batch
        batch = get_next_batch(tracking_data)
        
        if not batch:
            all_indexed = True
            break  # No more files to process
        
        # Index the batch
        success = index_batch(batch, tracking_data)
        batch_count += 1
        
        # Update progress
        indexed_count = len(tracking_data["indexed_files"])
        remaining = total_files - indexed_count
        print(f"\nProgress: {indexed_count}/{total_files} files indexed, {remaining} remaining")
        
        # Break if there was an error
        if not success:
            print("Stopping due to indexing error")
            break
            
        # Check if we're done
        if remaining <= 0:
            all_indexed = True
            break
            
        # Take a break to let CPU cool down
        print(f"Batch {batch_count} complete. Waiting for CPU to cool down ({cooldown_seconds} seconds)...")
        time.sleep(cooldown_seconds)
    
    print("\nIndexing process complete!")
    print(f"Total files indexed: {len(tracking_data['indexed_files'])}/{total_files}")
    
    # Only run the test search if all files were successfully indexed
    if all_indexed:
        print("\nAll files have been successfully indexed.")
        print("Running test search...")
        time.sleep(3)  # Give a moment for the index to fully settle
        run_test_search()
    else:
        print("\nNot all files were indexed. Skipping test search.")
        print("You can run the script again to continue indexing.")

# Main execution
if __name__ == "__main__":
    # process_all_files()
    run_test_search()
