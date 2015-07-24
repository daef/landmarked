var LibraryFileHelper = {
	Alert: function(msgptr) {
		window.alert(Pointer_stringify(msgptr));
	},
	CopyFileData: function(filenameptr, dstptr) {
		var filename = Pointer_stringify(filenameptr);
		var filedata = window.filedata[filename];
		var dataHeap = new Uint8Array(HEAPU8.buffer, dstptr, filedata.byteLength);
		//copy from JS to emscripten heap
		dataHeap.set(new Uint8Array(filedata));
		delete window.filedata[filename];
	},
	GetFileDataLength: function(filenameptr) {
		var filename = Pointer_stringify(filenameptr);
		return window.filedata[filename].byteLength;
	},
	DownloadFile: function(filenameptr, ptr, len) {
		saveAs(new Blob([HEAPU8.buffer.slice(ptr, ptr+len)]), Pointer_stringify(filenameptr));
	}
};

mergeInto(LibraryManager.library, LibraryFileHelper);
