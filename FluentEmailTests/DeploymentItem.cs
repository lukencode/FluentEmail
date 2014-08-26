namespace FluentEmailTests
{
	/*
	 * Created by SharpDevelop.
	 * User: Dr. Hector Diaz
	 * Date: 24/08/2014
	 * Time: 12:51 a.m.
	 * 
	 */

	using System;
	using System.IO;
	using System.Reflection;

	/// <summary>
	///  Create by Alexander Pacha as improvement of Matthew's code.
	/// Source: http://stackoverflow.com/questions/9378276/nunit-deploymentitem
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	public class DeploymentItem : Attribute
	{
		/// <summary>
		/// Marks an item to be relevant for a unit-test and copies it to deployment-directory for this unit-test.
		/// </summary>
		/// <param name="fileProjectRelativePath">The project-relative path to a file or a folder that will be copied into the deployment-directory of this unit-test.</param>
		public DeploymentItem(string fileProjectRelativePath)
		{
			// Escape input-path to correct back-slashes for Windows
			string filePath = fileProjectRelativePath.Replace("/", "\\");

			// Look up, where we are right now
			DirectoryInfo environmentDir = new DirectoryInfo(Environment.CurrentDirectory);

			// Get the full item-path of the deployment item
			string itemPath = new Uri(Path.Combine(environmentDir.Parent.Parent.FullName, filePath)).LocalPath;

			// Get the target-path where to copy the deployment item to
			string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			// Assemble the target path
			string itemPathInBin = new Uri(Path.Combine(binFolderPath, filePath)).LocalPath;

			// Decide whether it's a file or a folder
			if (File.Exists(itemPath)) // It's a file
			{
				// If it already exists, remove it
				if (File.Exists(itemPathInBin))
				{
					File.Delete(itemPathInBin);
				}

				// Assemble the parent folder path (because the item might be in multiple sub-folders.
				string parentFolderPathInBin = new DirectoryInfo(itemPathInBin).Parent.FullName;

				// If the target directory does not exist, create it
				if (!Directory.Exists(parentFolderPathInBin))
				{
					Directory.CreateDirectory(parentFolderPathInBin);
				}

				// If the source-file exists, copy it to the destination
				if (File.Exists(itemPath))
				{
					File.Copy(itemPath, itemPathInBin);
				}
			}
			else if (Directory.Exists(itemPath)) // It's a folder
			{
				// If it already exists, remove it
				if (Directory.Exists(itemPathInBin))
				{
					Directory.Delete(itemPathInBin, true);
				}

				// If the source-directory exists, copy it to the destination
				if (Directory.Exists(itemPath))
				{
					// Create target directory
					Directory.CreateDirectory(itemPathInBin);

					// Now Create all of the sub-directories
					foreach (string dirPath in Directory.GetDirectories(itemPath, "*", SearchOption.AllDirectories))
					{
						Directory.CreateDirectory(dirPath.Replace(itemPath, itemPathInBin));
					}

					//Copy all the files & Replaces any files with the same name
					foreach (string newPath in Directory.GetFiles(itemPath, "*.*", SearchOption.AllDirectories))
					{
						File.Copy(newPath, newPath.Replace(itemPath, itemPathInBin), true);
					}
				}
			}
		}
	}
}