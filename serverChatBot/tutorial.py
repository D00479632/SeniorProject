import getpass
import os
from langchain_core.messages import HumanMessage
from langchain_core.messages import AIMessage

if not os.environ.get("GROQ_API_KEY"):
  os.environ["GROQ_API_KEY"] = getpass.getpass("Enter API key for Groq: ")

from langchain.chat_models import init_chat_model

model = init_chat_model("llama3-8b-8192", model_provider="groq")

print(model.invoke([HumanMessage(content="Hi! I'm Bob")]))

# The model on its own does not have any concept of state. For example, if you ask a followup question:
print(model.invoke([HumanMessage(content="What's my name?")]))

# Let's take a look at the example LangSmith trace
# Let's pass the entire conversation history
print(model.invoke(
    [
        HumanMessage(content="Hi! I'm Bob"),
        AIMessage(content="Hello Bob! How can I assist you today?"),
        HumanMessage(content="What's my name?"),
    ]
))

