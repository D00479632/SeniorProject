from ollama import chat
from marqo import Client
import os  # Import os module to interact with the file system

question = "What gifts does Clint love?"
question = "Can you plant strawberries in the spring?"
question = "What is the price of corn seeds?"
question = "Give me a list of people that like Diamond."
def get_ollama_response(question, context=""):
    print("This is the context: ", context)
    question2 = "What gifts does Clint love?"
    context2 = get_marqo_context(question2)
    question3 = "Give me a list of people that like Amethyst."
    context3 = get_marqo_context(question3)
    messages = [
        {"role": "system", "content": """
        ALL RESPONSES SHOULD ONLY PERTAIN TO THE STARDEW VALLEY VIDEO GAME, NOT REAL LIFE.
        You are a Stardew Valley game assistant. Provide answers that are strictly relevant to the question asked.
        You use the information text to answer the user better.
        Avoid unnecessary context or elaboration. Focus solely on the question and provide a direct answer. """},
        {"role": "user", "content": f"Given this information: {context2} please answer the following question: {question2}"}, 
        {"role": "assistant", "content": "Clint loves: Amethyst, Aquamarine, Artichoke Dip, Emerald, Fiddlehead Risotto, Gold Bar, Iridium Bar, Jade, Omni Geode, Ruby, Topaz"}, 
        {"role": "user", "content": f"Given this information: {context3} please answer the following question: {question3}"}, 
        {"role": "assistant", "content": "Like  Alex •  Caroline •  Demetrius •  Elliott •  Evelyn •  George •  Gus •  Haley •  Harvey •  Jas •  Jodi •  Kent •  Krobus •  Leo •  Lewis •  Marnie •  Maru •  Pam •  Penny •  Robin •  Sam •  Sandy •  Sebastian •  Shane •  Vincent •  Willy •  Wizard"}, 
        {"role": "user", "content": f"Given this information: {context} please answer the following question: {question}"},
    ]
    response = chat("gemma3:4b", messages=messages)
    return response['message']['content']

def get_marqo_context(question):
    index_name = 'stardew-valley-data' 
    # Set up Marqo Client
    mq = Client(url='http://localhost:8882')
    
    print("Searching marqo")
    # Perform search on Marqo index
    results = mq.index(index_name).search(
        q=question,
        limit=2
    )

    # Prepare context
    context = ''
    # Reverse the hits to put the most relevant source at the end
    for i, hit in enumerate(reversed(results['hits'])):
        title = hit['Title']
        text = hit['Description']
        context += f"Source {i + 1}) {title} || {text} \n"
    return context

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


# Final LLM call with context
context = get_marqo_context(question)
final_response = get_ollama_response(question, context)
print("LLM & Marqo Response:", final_response)
