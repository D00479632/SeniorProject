# For original tutorial see: https://github.com/marqo-ai/marqo-tutorials/blob/main/starter-guides/rag/rag_open_source.py

#####################################################
### STEP 1. Run Marqo
#####################################################

"""
docker pull marqoai/marqo:latest
docker rm -f marqo
docker run --name marqo -it -p 8882:8882 marqoai/marqo:latest

ollama serve
ollama pull llama3.2:1b
"""

#####################################################
### STEP 2. Set Up Ollama LLM
#####################################################

from ollama import chat

# Define the question to be asked
question = "Who won gold in the women's 100 metre race at the Paris Olympics 2024?"

def get_ollama_response(question, context=""):
    messages = [
        {"role": "system", "content": "You are a helpful assistant."},
        {"role": "user", "content": f"{context} {question}"},
    ]
    response = chat("llama3.2:1b", messages=messages)
    return response['message']['content']

# Get the initial LLM response
first_response = get_ollama_response(question)
print("Just LLM Response:", first_response)

#####################################################
### STEP 3. Define Documents to Perform RAG
#####################################################

DOCUMENTS = [
    {
        '_id': '1',
        'date': '2024-08-03',
        'website': 'www.bbc.com',
        'Title': "Alfred storms to Olympic 100m gold in Paris.",
        'Description': "Julien Alfred stormed to the women's 100m title at Paris 2024 to make history as St Lucia's first Olympic medallist. As the rain teemed down at a raucous Stade de France, Alfred, 23, dominated the final and sealed victory by a clear margin in a national record 10.72 seconds. American world champion Sha'Carri Richardson took silver in 10.87, with compatriot Melissa Jefferson (10.92) third. Great Britain's Daryll Neita finished four-hundredths of a second off the podium in fourth, crossing the line in 10.96."
    }
]

#####################################################
### STEP 4. Use Marqo to Perform RAG
#####################################################

from marqo import Client

# Your index name
index_name = 'news-index-open-source'

# Set up Marqo Client
mq = Client(url='http://localhost:8882')

# Create index (delete if it exists)
try:
    mq.index(index_name).delete()
except:
    pass

mq.create_index(index_name)

# Index documents
mq.index(index_name).add_documents(DOCUMENTS, tensor_fields=["Title", "Description"])

# Perform search on Marqo index
results = mq.index(index_name).search(
    q=question,
    filter_string="date:2024-08-03",
    limit=5
)

# Print Marqo search results
print("Marqo Search Results:", results)

# Prepare context for LLM
context = ''
for i, hit in enumerate(results['hits']):
    title = hit['Title']
    text = hit['Description']
    context += f"Source {i + 1}) {title} || {' '.join(text.split()[:60])}... \n"

# Final LLM call with context
final_response = get_ollama_response(question, context)
print("LLM & Marqo Response:", final_response)

