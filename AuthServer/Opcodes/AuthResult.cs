/* MIT License
 * 
 * Copyright (c) 2019 Stanislaw Schlosser <https://github.com/0x2aff>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace WoWCore.AuthServer.Opcodes
{
    public enum AuthResult : byte
    {
        /// <summary>
        ///     Successful.
        /// </summary>
        Success = 0x00,

        /// <summary>
        ///     Unable to connect.
        /// </summary>
        Unknown0 = 0x01,

        /// <summary>
        ///     Unable to connect.
        /// </summary>
        Unknown1 = 0x02,

        /// <summary>
        ///     This account has been closed and is no longer available for use.
        /// </summary>
        Banned = 0x03,

        /// <summary>
        ///     The information you have entered is not valid.
        /// </summary>
        UnknownAccount = 0x04,

        /// <summary>
        ///     The information you have entered is not valid.
        /// </summary>
        IncorredPassword = 0x05,

        /// <summary>
        ///     This account is already logged in.
        /// </summary>
        AlreadyOnline = 0x06,

        /// <summary>
        ///     You have used up your prepaid time for this account.
        /// </summary>
        NoTime = 0x07,

        /// <summary>
        ///     Could not log in.
        /// </summary>
        DatabaseBusy = 0x08,

        /// <summary>
        ///     Unable to validate game version.
        /// </summary>
        VersionInvalid = 0x09,

        /// <summary>
        ///     Downloading.
        /// </summary>
        VersionUpdate = 0x0A,

        /// <summary>
        ///     Unable to connect.
        /// </summary>
        InvalidServer = 0x0B,

        /// <summary>
        ///     This account has been temporarily suspended.
        /// </summary>
        Suspended = 0x0C,

        /// <summary>
        ///     Unable to connect.
        /// </summary>
        NoAccess = 0x0D,

        /// <summary>
        ///     Connected.
        /// </summary>
        SuccessSurvey = 0x0E,

        /// <summary>
        ///     Acces to this account has been blocked by parental controls.
        /// </summary>
        ParentControl = 0x0F,

        /// <summary>
        ///     You have applied a lock to your account.
        /// </summary>
        LockedEnforced = 0x10,

        /// <summary>
        ///     Your trial subscription has expired.
        /// </summary>
        TrialEnded = 0x11,

        /// <summary>
        ///     This account is now attached to a Battle.net account.
        /// </summary>
        UseBattleNet = 0x12
    }
}