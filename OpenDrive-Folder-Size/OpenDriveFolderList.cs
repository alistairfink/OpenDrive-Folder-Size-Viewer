using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDrive_Folder_Size
{
    public class OpenDriveFolderList
    {
        public string DirUpdateTime { get; set; }
        public string Name { get; set; }
        public string ParentFolderID { get; set; }
        public string DirectFolderLink { get; set; }
        public int ResponseType { get; set; }
        public List<Folder> Folders { get; set; }
        public List<File> Files { get; set; }
    }
    public class Folder
    {
        public string FolderID { get; set; }
        public string Name { get; set; }
        public int DateCreated { get; set; }
        public int DirUpdateTime { get; set; }
        public int Access { get; set; }
        public int DateModified { get; set; }
        public string Shared { get; set; }
        public int ChildFolders { get; set; }
        public string Link { get; set; }
        public string Encrypted { get; set; }
    }

    public class File
    {
        public string FileId { get; set; }
        public string Name { get; set; }
        public int GroupID { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
        public string Views { get; set; }
        public string Version { get; set; }
        public string Downloads { get; set; }
        public string DateModified { get; set; }
        public string Access { get; set; }
        public string FileHash { get; set; }
        public string Link { get; set; }
        public string DownloadLink { get; set; }
        public string StreamingLink { get; set; }
        public string TempStreamingLink { get; set; }
        public string ThumbLink { get; set; }
        public string Password { get; set; }
        public int EditOnline { get; set; }
    }
}
