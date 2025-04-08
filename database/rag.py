from ollama import chat
from marqo import Client

def get_ollama_response(question, context=""):
    index_name = "stardew-valley-db"
    question2 = "What gifts does Clint love?"
    context2 = get_marqo_context(question2, index_name)
    question3 = "Give me a list of people that like Amethyst."
    context3 = get_marqo_context(question3, index_name)
    messages = [
        {"role": "system", "content": """
        ALL RESPONSES SHOULD ONLY PERTAIN TO THE STARDEW VALLEY VIDEO GAME, NOT REAL LIFE.
        You are a Stardew Valley game assistant. Provide answers that are strictly relevant to the question asked.
        You use the information text to answer the user better.
        Avoid unnecessary context or elaboration. Focus solely on the question and provide a direct answer. """},
        {"role": "user", "content": f"Given this information: {context2} please answer the following question: {question2}"}, 
        {"role": "assistant", "content": "Clint loves: Amethyst, Aquamarine, Artichoke Dip, Emerald, Fiddlehead Risotto, Gold Bar, Iridium Bar, Jade, Omni Geode, Ruby, Topaz"}, 
        {"role": "user", "content": f"Given this information: {context3} please answer the following question: {question3}"}, 
        {"role": "assistant", "content": "Like:  Alex •  Caroline •  Demetrius •  Elliott •  Evelyn •  George •  Gus •  Haley •  Harvey •  Jas •  Jodi •  Kent •  Krobus •  Leo •  Lewis •  Marnie •  Maru •  Pam •  Penny •  Robin •  Sam •  Sandy •  Sebastian •  Shane •  Vincent •  Willy •  Wizard"}, 
        {"role": "user", "content": f"Given this information: {context} please answer the following question: {question}"},
    ]
    response = chat("gemma3:4b", messages=messages)
    return response['message']['content']

def get_marqo_context(question, index_name):
    # Set up Marqo Client
    mq = Client(url='http://localhost:8882')
    
    print(f"Searching marqo index: {index_name}")
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

def compare_databases(question):
    # Test with first database
    print("\n=== Testing 'stardew-valley-data' database ===")
    context1 = get_marqo_context(question, 'stardew-valley-data')
    response1 = get_ollama_response(question, context1)
    print("Response from 'stardew-valley-data':", response1)
    
    # Test with second database
    print("\n=== Testing 'stardew-valley-db' database ===")
    context2 = get_marqo_context(question, 'stardew-valley-db')
    response2 = get_ollama_response(question, context2)
    print("Response from 'stardew-valley-db':", response2)
    
    return {
        'stardew-valley-data': {'context': context1, 'response': response1},
        'stardew-valley-db': {'context': context2, 'response': response2}
    }

# Test questions to compare databases
test_questions = [
    "Give me a list of people that like Diamond.",
    "What gifts does Clint love?",
    "What is Clint's regular schedule?",
    "What are the best crops to grow in Spring?",
    "How do I get a Prismatic Shard?"
]

# Run comparison tests
print("=== DATABASE COMPARISON TESTS ===")
results = {}
for question in test_questions:
    print(f"\n\n=== Testing question: '{question}' ===")
    results[question] = compare_databases(question)

print("\n=== COMPARISON COMPLETE ===")
