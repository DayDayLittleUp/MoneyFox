namespace MoneyFox.Core.DbBackup
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Common.Exceptions;

    /// <summary>
    ///     Provides Backup and Restore operations.
    /// </summary>
    public interface IOneDriveBackupService
    {
        /// <summary>
        ///     Information about logged user.
        /// </summary>
        UserAccount UserAccount { get; set; }

        /// <summary>
        ///     Login user.
        /// </summary>
        /// <exception cref="BackupAuthenticationFailedException">Thrown when the user couldn't be logged in.</exception>
        Task LoginAsync();

        /// <summary>
        ///     Logout user.
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        ///     Uploads a copy of the current database.
        /// </summary>
        /// <param name="dataToUpload">Stream of data to upload.</param>
        /// <returns>Returns a TaskCompletionType which indicates if the task was successful or not</returns>
        /// <exception cref="BackupOperationCanceledException">Thrown when the user couldn't be logged in.</exception>
        Task<bool> UploadAsync(Stream dataToUpload);

        /// <summary>
        ///     Restores the file with the passed name
        /// </summary>
        /// <returns>TaskCompletionType which indicates if the task was successful or not</returns>
        /// <exception cref="NoBackupFoundException">Thrown when no backup with the right name is found.</exception>
        /// <exception cref="BackupAuthenticationFailedException">Thrown when the user couldn't be logged in.</exception>
        Task<Stream> RestoreAsync();

        /// <summary>
        ///     Gets a list with all the filenames who are available in the backup folder.     The name of the backupfolder is
        ///     defined in the Constants.
        /// </summary>
        /// <returns>A list with all filenames.</returns>
        /// <exception cref="BackupAuthenticationFailedException">Thrown when the user couldn't be logged in.</exception>
        Task<List<string>> GetFileNamesAsync();

        /// <summary>
        ///     Get's the modification date for the existing backup.
        /// </summary>
        /// <returns>Returns the date of the last backup</returns>
        /// <exception cref="BackupAuthenticationFailedException">Thrown when the user couldn't be logged in.</exception>
        Task<DateTime> GetBackupDateAsync();
    }
}
