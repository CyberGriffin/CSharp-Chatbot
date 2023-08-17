"""
Script to search indexed documents.
"""
import os
from langchain.vectorstores import FAISS
from langchain.embeddings.openai import OpenAIEmbeddings

embedding_model = os.getenv("OPENAI_EMBEDDING_DEPLOYMENT_NAME")
embedding = OpenAIEmbeddings(model=embedding_model, chunk_size=1, disallowed_special=())

base_path = os.path.dirname(os.path.normpath(os.getcwd()))
faiss_path = f"{base_path}\\data\\faiss"

db = FAISS.load_local(faiss_path, embedding)

def search_documents(query: str) -> str:
    """
    Run the QA chain.
    """
    result = db.similarity_search_with_score(query, k=2)
    relevancy_score = result[0][1]
    result = "" if relevancy_score < 0.7 else result[0][0].page_content

    print(f"Relevancy score: {relevancy_score}")
    print(f"Result: {result}")
    return result
