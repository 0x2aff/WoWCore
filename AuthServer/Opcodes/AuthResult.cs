/*
 *  WoWCore - World of Warcraft 1.12 Server
 *  Copyright (C) 2017 exceptionptr
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *  
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
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
