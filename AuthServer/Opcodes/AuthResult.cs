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
        Success = 0x00,
        Unknown0 = 0x01,
        Unknown1 = 0x02,
        Banned = 0x03,
        UnknownAccount = 0x04,
        IncorrectPassword = 0x05,
        AlreadyOnline = 0x06,
        NoTime = 0x07,  
        DatabaseBusy = 0x08,
        VersionInvalid = 0x09,
        VersionUpdate = 0x0A,
        InvalidServer = 0x0B,
        Suspended = 0x0C,
        NoAccess = 0x0D,
        SuccessSurvey = 0x0E,
        ParentControl = 0x0F,
        LockedEnforced = 0x10,
        TrialEnded = 0x11,
        UseBattleNet = 0x12
    }
}
