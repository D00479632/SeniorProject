import marqo
import pprint

'''
For this code to run I need to run marqo with docker with these commands:

docker pull marqoai/marqo:latest
docker rm -f marqo
docker run --name marqo -it -p 8882:8882 marqoai/marqo:latest

If you have done that you can:

docker start marqo

Check if its running:

docker ps

To stop it:

docker stop marqo

Now docker ps doesn't show the container 
'''

# Create a Marqo client
# mq is the client that wraps themarqo API
mq = marqo.Client(url="http://localhost:8882")

# Housekeeping - Delete the index if it already exists
try:
    mq.index("my-first-index").delete()
except:
    pass

# Create the index
'''
create_index() creates a new index with default settings. We optionally specify the model to be 
hf/e5-base-v2 which is also the default model. Other models are supported by Marqo. See link for 
the full list of models. Experimentation with different models is often required to achieve the 
best retrieval for your specific use case. Different models also offer a tradeoff between inference 
speed and relevancy.
https://docs.marqo.ai/latest/models/marqo/list-of-models/
'''
mq.create_index("my-first-index", model="hf/e5-base-v2")

# Add documents to the index
# add_documents() takes a list of documents, represented as python dicts, for indexing.
mq.index("my-first-index").add_documents(
    [
        {
            "Title": "The Travels of Marco Polo",
            "Description": "A 13th-century travelogue describing Polo's travels",
        },
        {
            "Title": "Extravehicular Mobility Unit (EMU)",
            "Description": "The EMU is a spacesuit that provides environmental protection, "
            "mobility, life support, and communications for astronauts",
            # You can optionally set a document's ID with the special _id field. Otherwise, Marqo will generate one.
            "_id": "article_591",
        },
    ],
    # The tensor_fields parameter specifies which fields should be indexed as tensor fields, and 
    # searchable with vector search.
    tensor_fields=["Description"],
)

# Obtain results for a specific query
results = mq.index("my-first-index").search(
    q="What is the best outfit to wear on the moon?"
)

#pprint.pprint(results)
'''
{'hits': [{'Description': 'The EMU is a spacesuit that provides environmental '
                          'protection, mobility, life support, and '
                          'communications for astronauts',
           'Title': 'Extravehicular Mobility Unit (EMU)',
           '_highlights': [{'Description': 'The EMU is a spacesuit that '
                                           'provides environmental protection, '
                                           'mobility, life support, and '
                                           'communications for astronauts'}],
           '_id': 'article_591',
           '_score': 0.8302064702029864},
          {'Description': "A 13th-century travelogue describing Polo's travels",
           'Title': 'The Travels of Marco Polo',
           '_highlights': [{'Description': 'A 13th-century travelogue '
                                           "describing Polo's travels"}],
           '_id': 'dce8ec0f-0496-48e7-abae-96ac027ba925',
           '_score': 0.7665057498844796}],
 'limit': 10,
 'offset': 0,
 'processingTimeMs': 1868,
 'query': 'What is the best outfit to wear on the moon?'}

 Explanation:

  * Each hit corresponds to a document that matched the search query
  * They are ordered from most to least matching. This can be seen in the _score field
  * limit is the maximum number of hits to be returned. This can be set as a parameter during search
  * Each hit has a _highlights field. This was the part of the document that matched the query the best

'''

# Retrieve a document by ID.
result = mq.index("my-first-index").get_document(document_id="article_591")
# Note that by adding the document using add_documents again using the same _id will cause a document to be updated.

# Get Index Stats
results = mq.index("my-first-index").get_stats()

# Lexical Search
result = mq.index("my-first-index").search("marco polo", search_method="LEXICAL")

# Hybrid Search
result = mq.index("my-first-index").search("marco polo", search_method="HYBRID")

# Multimodal and Cross Modal Search
# To power image and text search, Marqo allows users to plug and play with CLIP models from Hugging Face. 
# Check: https://docs.marqo.ai/latest/quickstart/marqo/getting-started/

# Delete documents
results = mq.index("my-first-index").delete_documents(
    ids=["article_591", "article_602"]
)

# Delete index
results = mq.index("my-first-index").delete()
