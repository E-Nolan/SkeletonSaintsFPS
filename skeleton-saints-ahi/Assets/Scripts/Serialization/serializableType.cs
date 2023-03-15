using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Runtime.Serialization;

[System.Serializable]
public class serializableType
{
	[SerializeField]
	private string m_Name;

	public string Name
	{
		get { return m_Name; }
	}

	[SerializeField]
	private string m_AssemblyQualifiedName;

	public string AssemblyQualifiedName
	{
		get { return m_AssemblyQualifiedName; }
	}

	private System.Type m_SystemType;
	public System.Type SystemType
	{
		get
		{
			if (m_SystemType == null)
			{
				GetSystemType();
			}
			return m_SystemType;
		}
	}

	private void GetSystemType()
	{
		m_SystemType = System.Type.GetType(m_AssemblyQualifiedName);
	}

	public serializableType(System.Type _SystemType)
	{
		m_SystemType = _SystemType;
		m_Name = _SystemType.Name;
		m_AssemblyQualifiedName = _SystemType.AssemblyQualifiedName;
	}

	public override bool Equals(System.Object obj)
	{
		serializableType temp = obj as serializableType;
		if ((object)temp == null)
		{
			return false;
		}
		return this.Equals(temp);
	}

	public bool Equals(serializableType _Object)
	{
		//return m_AssemblyQualifiedName.Equals(_Object.m_AssemblyQualifiedName);
		return _Object.SystemType.Equals(SystemType);
	}

	public static bool operator ==(serializableType a, serializableType b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}

		return a.Equals(b);
	}

	public static bool operator !=(serializableType a, serializableType b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return SystemType != null ? SystemType.GetHashCode() : 0;
	}
}