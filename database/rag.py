from ollama import chat
from marqo import Client
import os  # Import os module to interact with the file system

question = "Does Clint like Holly?"
def get_ollama_response(question, context=""):
    messages = [
        {"role": "system", "content": """
        ALL RESPONSES SHOULD ONLY PERTAIN TO THE STARDEW VALLEY VIDEO GAME, NOT REAL LIFE.
        You use the text to answer the user better.
        You are a Stardew Valley game assistant. Provide answers that are strictly relevant to the question asked.
        Avoid unnecessary context or elaboration. Focus solely on the question and provide a direct answer.
        For example, if asked 'can I plant tomatoes in the spring?', respond with 'tomatoes only grow in summer, so you cannot plant them in spring.'"""},
        {"role": "user", "content": f"{context} {question}"},
    ]
    response = chat("gemma3:4b", messages=messages)
    return response['message']['content']

'''
first_response = get_ollama_response(question)

print("Just LLM Response:", first_response)
'''


# Define the path to the directory containing your text files
filestore_path = 'Scraper/txt/crops'
index_name = 'stardew-valley-data'

'''
# Initialize the DOCUMENTS list
DOCUMENTS = []

print("Getting documents ready")
# Loop through each file in the filestore directory
for filename in os.listdir(filestore_path):
    if filename.endswith('.txt'):  # Check if the file is a text file
        with open(os.path.join(filestore_path, filename), 'r') as file:
            filecontent = file.read()  # Read the content of the file
            DOCUMENTS.append({
                'Title': filename, 
                'Description': filecontent
            })




# Create index (delete if it exists)
try:
    mq.index(index_name).delete()
except:
    pass

mq.create_index(index_name)

# Index documents
# Tensor fields are for similarity search
mq.index(index_name).add_documents(DOCUMENTS, tensor_fields=["Title", "Description"])
'''

# Set up Marqo Client
mq = Client(url='http://localhost:8882')

# Perform search on Marqo index
results = mq.index(index_name).search(
    q=question,
    limit=1
)

print("Marqo Search Results:", results)

# Prepare context for LLM
print("Getting context")
context = ''
for i, hit in enumerate(results['hits']):
    title = hit['Title']
    text = hit['Description']
    # Include the full content of the text file in the context
    context += f"Source {i + 1}) {title} || {text} \n"

#print("This is the context from marqo: ", context)

# Final LLM call with context
final_response = get_ollama_response(question, context)
print("LLM & Marqo Response:", final_response)
