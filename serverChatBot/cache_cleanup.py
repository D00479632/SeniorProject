from huggingface_hub import scan_cache_dir
import shutil
import os

def print_cache_info():
    """Print information about currently cached models"""
    cache_info = scan_cache_dir()
    if not cache_info.repos:
        print("No models found in cache.")
        return False
    
    total_size = 0
    print("\nCurrently cached models:")
    for repo in cache_info.repos:
        size_mb = repo.size_on_disk / 1024 / 1024
        total_size += size_mb
        print(f"- {repo.repo_id} ({size_mb:.2f} MB)")
    print(f"\nTotal cache size: {total_size:.2f} MB")
    return True

def clear_cache():
    """Clear all models from the cache with confirmation"""
    if not print_cache_info():
        return
    
    confirmation = input("\nAre you sure you want to delete ALL cached models? (yes/no): ")
    if confirmation.lower() != 'yes':
        print("Operation cancelled.")
        return
    
    try:
        print("\nDeleting cache...")
        # Get cache directory path
        cache_info = scan_cache_dir()
        for repo in cache_info.repos:
            if os.path.exists(repo.repo_path):
                shutil.rmtree(repo.repo_path)
                print(f"Deleted: {repo.repo_id}")
        print("Cache successfully cleared!")
    except Exception as e:
        print(f"Error while clearing cache: {e}")
        print("You might need to manually delete the cache directory or run with administrator privileges.")

if __name__ == "__main__":
    clear_cache()