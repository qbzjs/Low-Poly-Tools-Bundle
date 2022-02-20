using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : MonoBehaviour
{
	public delegate void NetworkMethod(string[] parameters);

	[SerializeField] private string hostAddress = "127.0.0.1";
	[SerializeField] private string username = "Dummy";
	[SerializeField] private string password = "GEHEIM";
	[SerializeField] private bool registerUser = false;
	private Dictionary<string, string> parameterDictionary = null;
	private HashSet<UnityWebRequest> runningRequests = null;
	private List<UnityWebRequest> deleteRequests = null;
	private Dictionary<string, NetworkMethod> listeners = null;

	private void Awake()
	{
		parameterDictionary = new Dictionary<string, string>();
		runningRequests = new HashSet<UnityWebRequest>();
		deleteRequests = new List<UnityWebRequest>();
		listeners = new Dictionary<string, NetworkMethod>();
	}

	private void Start()
	{
		RegisterListener(Register);
		RegisterListener(Login);
		RegisterListener(Logout);

		if(registerUser)
		{
			SendRequest(Register, new KeyValuePair<string, string>("Username", username),
				new KeyValuePair<string, string>("Password", password),
				new KeyValuePair<string, string>("RepeatPassword", password));
		}
		else
		{
			SendRequest(Login, new KeyValuePair<string, string>("Username", username),
				new KeyValuePair<string, string>("Password", password));
		}
	}

	private void Update()
	{
		deleteRequests.Clear();
		foreach(UnityWebRequest request in runningRequests)
		{
			if(request.isDone)
			{
				deleteRequests.Add(request);

				// DEBUG
				// Debug.Log(request.downloadHandler.text);

				string[] reply = request.downloadHandler.text.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
				if(reply.Length != 2)
				{
					Debug.LogError("Invalid Server Reply '" + request.downloadHandler.text + "' in NetworkController!");
					continue;
				}

				if(listeners.ContainsKey(reply[0]))
				{
					listeners[reply[0]](reply[1].Split('|', System.StringSplitOptions.RemoveEmptyEntries));
				}
				else
				{
					Debug.LogError("Invalid Method Name '" + reply[0] + "' in NetworkController!");
					continue;
				}
			}
		}
		foreach(UnityWebRequest request in deleteRequests)
		{
			// https://github.com/LastAbyss/SimpleGraphQL-For-Unity/issues/28
			request.uploadHandler.Dispose();
			request.downloadHandler.Dispose();
			request.Dispose();

			runningRequests.Remove(request);
		}
	}

	public void SendRequest(NetworkMethod callback, params KeyValuePair<string, string>[] parameters)
	{
		parameterDictionary.Clear();
		parameterDictionary.Add("MethodName", callback.Method.Name);
		foreach(KeyValuePair<string, string> parameter in parameters)
		{
			parameterDictionary.Add(parameter.Key, parameter.Value);
		}

		UnityWebRequest request = UnityWebRequest.Post(hostAddress, parameterDictionary);
		request.SendWebRequest();
		runningRequests.Add(request);
	}

	public void Register(string[] parameters)
	{
		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Registration successful");
		}
		else
		{
			Debug.Log("Registration failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Login(string[] parameters)
	{
		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Login successful");
		}
		else
		{
			Debug.Log("Login failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Logout(string[] parameters)
	{
		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Logout successful");
		}
		else
		{
			Debug.Log("Logout failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void RegisterListener(NetworkMethod networkMethod)
	{
		listeners.Add(networkMethod.Method.Name, networkMethod);
	}
}
