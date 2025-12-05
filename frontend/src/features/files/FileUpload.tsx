import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { api } from '../../lib/axios';
import { API_ROUTES } from '../../constants';

export const FileUpload = () => {
  const [file, setFile] = useState<File | null>(null);

  const uploadMutation = useMutation({
    mutationFn: async (file: File) => {
      const formData = new FormData();
      formData.append('file', file);
      const { data } = await api.post(API_ROUTES.FILES.UPLOAD, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      return data;
    },
    onSuccess: () => {
      setFile(null);
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (file) uploadMutation.mutate(file);
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <h2 className="text-xl font-bold mb-4">Upload File</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <input
            type="file"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
            className="w-full px-3 py-2 border rounded-lg"
          />
        </div>
        <button
          type="submit"
          disabled={!file || uploadMutation.isPending}
          className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 disabled:opacity-50"
        >
          {uploadMutation.isPending ? 'Uploading...' : 'Upload'}
        </button>
        {uploadMutation.isSuccess && (
          <p className="text-green-600">File uploaded successfully!</p>
        )}
        {uploadMutation.isError && (
          <p className="text-red-600">Upload failed</p>
        )}
      </form>
    </div>
  );
};
