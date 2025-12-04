import { useState } from 'react';
import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import { userService } from './userService';
import { roleService } from '../roles/roleService';
import type { User } from '../../types';

interface UserFormProps {
  user?: User;
  onClose: () => void;
}

export const UserForm = ({ user, onClose }: UserFormProps) => {
  const queryClient = useQueryClient();
  const [formData, setFormData] = useState({
    username: user?.username || '',
    email: user?.email || '',
    password: '',
    isActive: user?.isActive ?? true,
    roleIds: user?.roles?.map((r) => r.id) || [],
  });

  const { data: roles = [] } = useQuery({
    queryKey: ['roles'],
    queryFn: roleService.getAll,
  });

  const mutation = useMutation({
    mutationFn: user
      ? (data: any) => userService.update(user.id, data)
      : userService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      onClose();
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate(formData);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 className="text-2xl font-bold mb-4">{user ? 'Edit User' : 'Create User'}</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Username</label>
            <input
              type="text"
              value={formData.username}
              onChange={(e) => setFormData({ ...formData, username: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Email</label>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          {!user && (
            <div>
              <label className="block text-sm font-medium mb-1">Password</label>
              <input
                type="password"
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>
          )}
          <div>
            <label className="block text-sm font-medium mb-1">Roles</label>
            <select
              multiple
              value={formData.roleIds}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  roleIds: Array.from(e.target.selectedOptions, (option) => option.value),
                })
              }
              className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {roles.map((role: any) => (
                <option key={role.id} value={role.id}>
                  {role.name}
                </option>
              ))}
            </select>
          </div>
          <div className="flex items-center">
            <input
              type="checkbox"
              checked={formData.isActive}
              onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
              className="mr-2"
            />
            <label className="text-sm font-medium">Active</label>
          </div>
          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={mutation.isPending}
              className="flex-1 bg-blue-500 text-white py-2 rounded-lg hover:bg-blue-600 disabled:opacity-50"
            >
              {mutation.isPending ? 'Saving...' : 'Save'}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="flex-1 bg-gray-300 text-gray-700 py-2 rounded-lg hover:bg-gray-400"
            >
              Cancel
            </button>
          </div>
          {mutation.isError && (
            <p className="text-red-500 text-sm">
              {(mutation.error as any)?.response?.data?.message || 'Failed to save user'}
            </p>
          )}
        </form>
      </div>
    </div>
  );
};
